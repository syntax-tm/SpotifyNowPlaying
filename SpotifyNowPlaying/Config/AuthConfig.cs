using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace SpotifyNowPlaying.Config
{
    public class AuthConfig
    {
        [JsonProperty("isImplicitGrant")]
        public bool IsImplicitGrant { get; set; }
        [JsonProperty("authCode")]
        public string AuthCode { get; set; }
        [JsonProperty("authState")]
        public string AuthState { get; set; }
        [JsonProperty("verifier")]
        public string Verifier { get; set; }
        [JsonProperty("challenge")]
        public string Challenge { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
        [JsonProperty("tokenType")]
        public string TokenType { get; set; }
        [JsonProperty("tokenResponse")]
        public PKCETokenResponse TokenResponse { get; set; }
    }
}
