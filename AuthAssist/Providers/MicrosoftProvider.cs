using AuthAssist.Providers.Models;

namespace AuthAssist.Providers
{
    // Well-known: https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration
    // Docs: https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc
    public class MicrosoftProvider(Settings settings, IAuthHandler authHandler)
        : OpenIdProviderBase, IMicrosoftProvider
    {
        public override string AuthUrl => "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";

        public override string ClientId => settings.GoogleIdp.ClientId;

        public override string ClientSecret => settings.GoogleIdp.ClientSecret;

        public override Settings Settings => settings;

        public override IAuthHandler AuthHandler => authHandler;
    }
}
