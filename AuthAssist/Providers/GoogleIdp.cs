namespace AuthAssist.Providers
{
    public class GoogleIdp(string clientId, string clientSecret)
    {
        public string ClientId { get; set; } = clientId;

        public string ClientSecret { get; set; } = clientSecret;
    }
}
