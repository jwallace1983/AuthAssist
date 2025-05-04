using System.Text.Json.Serialization;

namespace AuthAssist.Services.OpenId
{
    public class UserInfoResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("givenname")]
        public string FirstName { get; set; }

        [JsonPropertyName("given_name")]
        public string FirstNameAlternate { set => FirstName = value; }

        [JsonPropertyName("familyname")]
        public string LastName { get; set; }

        [JsonPropertyName("family_name")]
        public string LastNameAlternate { set => LastName = value; }

        public string Email { get; set; }
    }
}
