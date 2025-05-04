using System.Text.Json.Serialization;

namespace AuthAssist.Providers.Models
{
    public class UserInfoResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("givenname")]
        public string FirstName { get; set; }

        [JsonPropertyName("given_name")]
        public string FirstNameAlternate { set => this.FirstName = value; }

        [JsonPropertyName("familyname")]
        public string LastName { get; set; }

        [JsonPropertyName("family_name")]
        public string LastNameAlternate { set => this.LastName = value; }

        public string Email { get; set; }
    }
}
