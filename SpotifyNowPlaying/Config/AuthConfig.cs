using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace SpotifyNowPlaying.Config;

public class AuthConfig
{
    [JsonProperty("authCode")]
    public string AuthCode { get; set; }
    [JsonProperty("verifier")]
    public string Verifier { get; set; }
    [JsonProperty("challenge")]
    public string Challenge { get; set; }
    [JsonProperty("clientId")]
    public string ClientId { get; set; }
    [JsonProperty("callbackUrl")]
    public string CallbackUrl { get; set; }
    [JsonProperty("tokenResponse")]
    public PKCETokenResponse TokenResponse { get; set; }
}
