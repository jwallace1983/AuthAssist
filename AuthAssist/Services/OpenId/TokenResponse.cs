using System.Text.Json.Serialization;

namespace AuthAssist.Services.OpenId
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }
}
