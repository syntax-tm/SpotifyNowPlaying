using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SpotifyNowPlaying.Common
{
    public static class SpotifyClientHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SpotifyClientHelper));
        
        private const string TOKEN_CACHE_NAME = @"user.json";
        private const string REDIRECT_URL = @"http://localhost:5000/callback";
        private const string CALLBACK_URL = @"http://localhost:5000";
        private const string LISTENER_URL = @"http://*:5000/";
        private const string CLIENT_ID = @"c77562b58c1342598aef24a3efb28bce";
        private const string CODE_CHALLENGE_METHOD = @"S256";

        private static SpotifyClient _client;
        private static string _verifier;
        private static string _challenge;
        private static TaskCompletionSource<bool> _tcs;
        private static EmbedIOAuthServer _server;
        private static AuthorizationCodeTokenResponse _tokenResponse;
        
        public static SpotifyClient Client => _client;

        public static IAuthenticator Authenticator { get; set; }
        public static SpotifyClientConfig ClientConfig { get; set; }
        
        public static async Task Init()
        {
            (_verifier, _challenge) = PKCEUtil.GenerateCodes();

            _server = new EmbedIOAuthServer(new Uri(REDIRECT_URL), 5000);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, CLIENT_ID, LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = CODE_CHALLENGE_METHOD,
                CodeChallenge = _challenge,
                Scope = new List<string> { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackState, Scopes.UserReadRecentlyPlayed }
            };
            
            _tcs = new TaskCompletionSource<bool>();

            BrowserUtil.Open(request.ToUri());

            await _tcs.Task;
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
            log.Error($"Aborting authorization, error received: {error}.");
            log.Fatal("Login failed. See above for more information. Application exiting...");
            
            await _server.Stop();

            Environment.Exit(AppExitCode.LoginFailed);
        }
        
        private static async Task<bool> LoadConfig([NotNull] AuthorizationCodeResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            try
            {
                _tokenResponse = await new OAuthClient().RequestToken(
                    new TokenSwapTokenRequest(new Uri("http://localhost:5001/swap"), response.Code)
                );
                
                _client = new SpotifyClient(_tokenResponse.AccessToken);

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

        private static async Task Refresh()
        {
            if (!_tokenResponse.IsExpired) return;

            var newResponse = await new OAuthClient().RequestToken(
                new TokenSwapTokenRequest(new Uri("http://localhost:5001/swap"), _tokenResponse.RefreshToken)
            );
        }

        private static void SaveCreds(ImplictGrantResponse response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var configText = JsonConvert.SerializeObject(response, Formatting.None);

            IsolatedStorageManager.SaveFile(TOKEN_CACHE_NAME, configText);
        }

    }
}
