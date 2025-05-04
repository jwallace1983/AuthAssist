using AuthAssist.Providers.Models;

namespace AuthAssist.Providers
{
    public class GoogleProvider(Settings settings, IAuthHandler authHandler) : OpenIdProviderBase, IGoogleProvider
    {
        public const string WELL_KNOWN_CONFIG = "https://accounts.google.com/.well-known/openid-configuration";

        public override string AuthUrl => "https://accounts.google.com/o/oauth2/v2/auth";

        public override string ClientId => settings.GoogleIdp.ClientId;

        public override string ClientSecret => settings.GoogleIdp.ClientSecret;

        public override GoogleSettings ClientSettings => throw new System.NotImplementedException();

        public override Settings Settings => settings;

        public override IAuthHandler AuthHandler => authHandler;
    }
}
