namespace AuthAssist.Providers
{
    public class MicrosoftProvider(string clientId, string clientSecret)
    {
        public const string WELL_KNOWN = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
        // https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc
        public string ClientId { get; set; } = clientId;

        public string ClientSecret { get; set; } = clientSecret;
    }
}
