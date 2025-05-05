using AuthAssist.Services.OpenId;

namespace AuthAssist.Services.Microsoft
{
    // Well-known: https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration
    // Docs: https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc
    public class MicrosofAuthServive(Settings settings, IAuthHandler authHandler)
        : OpenIdAuthServiceBase, IMicrosoftAuthService
    {
        public override string Endpoint => "microsoft";

        public override string AuthUrl => "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";

        public override string TokenUrl => "https://login.microsoftonline.com/common/oauth2/v2.0/token";

        public override string UserInfoUrl => "https://graph.microsoft.com/oidc/userinfo";

        public override string ClientId => settings.MicrosoftIdp.ClientId;

        public override string ClientSecret => settings.MicrosoftIdp.ClientSecret;

        public override Settings Settings => settings;

        public override IAuthHandler AuthHandler => authHandler;

        public override AuthTypes AuthType => AuthTypes.Microsoft;
    }
}
