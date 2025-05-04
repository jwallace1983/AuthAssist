using System.Text.Json.Serialization;

namespace AuthAssist.Providers.Models
{
    public class UserInfoResponse
    {
        [JsonPropertyName("sub")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("given_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("family_name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Picture { get; set; }
    }
}
