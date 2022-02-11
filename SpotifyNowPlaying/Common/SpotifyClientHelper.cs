using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyNowPlaying.Config;

namespace SpotifyNowPlaying.Common
{
    public static class SpotifyClientHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SpotifyClientHelper));
        
        private const string TOKEN_CACHE_NAME = @"user.json";
        private const string REDIRECT_URL = @"http://localhost:5000/callback";
        private const string CLIENT_ID = @"c77562b58c1342598aef24a3efb28bce";
        private const string CODE_CHALLENGE_METHOD = @"S256";

        private static string _verifier;
        private static string _challenge;
        private static EmbedIOAuthServer _server;
        private static TaskCompletionSource<bool> _tcs;
        
        private static SpotifyClient _client;
        public static SpotifyClient Client => _client;

        public static async Task Init()
        {
            (_verifier, _challenge) = PKCEUtil.GenerateCodes(120);

            _server = new (new (REDIRECT_URL), 5000);
            await _server.Start();
            
            _server.AuthorizationCodeReceived += OnAuthReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(new (REDIRECT_URL), CLIENT_ID, LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = CODE_CHALLENGE_METHOD,
                CodeChallenge = _challenge,
                Scope = new [] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackState, Scopes.UserReadRecentlyPlayed }
            };
            
            _tcs = new ();
            
            BrowserUtil.Open(request.ToUri());

            await _tcs.Task.ConfigureAwait(false);
        }
        
        private static async Task OnAuthReceived(object sender, AuthorizationCodeResponse response)
        {
            try
            {
                await _server.Stop();
                
                var success = await LoadConfig(response);
                if (!success)
                {
                    _tcs.TrySetResult(false);
                    return;
                }
            
                _tcs.SetResult(true);
            }
            catch (Exception e)
            {
                log.Error($"An error occurred handling the auth response. {e.Message}");
        
                _tcs.SetResult(false);
            }
        }
        
        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            try
            {
                log.Error($"Aborting authorization, error received: {error}.");
                log.Fatal("Login failed. See above for more information. Application exiting...");

                await _server.Stop();
            }
            catch (Exception e)
            {
                log.Fatal($"An error occurred during application exit. {e.Message}", e);
            }
            finally
            {
                Environment.Exit(AppExitCode.LoginFailed);
            }
        }
        
        private static async Task<bool> LoadConfig([NotNull] AuthorizationCodeResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            try
            {
                var initialResponse = await new OAuthClient().RequestToken(
                    new PKCETokenRequest(CLIENT_ID, response.Code, new (REDIRECT_URL), _verifier)
                );
                
                var authenticator = new PKCEAuthenticator(CLIENT_ID, initialResponse);

                var config = SpotifyClientConfig.CreateDefault()
                    .WithAuthenticator(authenticator);

                _client = new (config);

                var authConfig = new AuthConfig
                {
                    Challenge = _challenge,
                    CallbackUrl = REDIRECT_URL,
                    Verifier = _verifier,
                    AuthCode = response.Code,
                    TokenResponse = initialResponse,
                    ClientId = CLIENT_ID
                };

                SaveCreds(authConfig);

                return true;
            }
            catch (APIException ae)
            {
                throw new SpotifyNowPlayingException(ae.Message, ae);
            }
            catch (Exception e)
            {
                throw new SpotifyNowPlayingException(e.Message, e);
            }
        }
        
        private static void SaveCreds(AuthConfig response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var configText = JsonConvert.SerializeObject(response, Formatting.None);

            IsolatedStorageManager.SaveFile(TOKEN_CACHE_NAME, configText);
        }

    }
}
