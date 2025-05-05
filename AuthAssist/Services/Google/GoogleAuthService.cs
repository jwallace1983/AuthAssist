using AuthAssist.Services.OpenId;

namespace AuthAssist.Services.Google
{
    // Well-known: https://accounts.google.com/.well-known/openid-configuration
    public class GoogleAuthService(Settings settings, IAuthHandler authHandler)
        : OpenIdAuthServiceBase, IGoogleAuthService
    {
        public override string Endpoint => "google";

        public override string AuthUrl => "https://accounts.google.com/o/oauth2/v2/auth";

        public override string TokenUrl => "https://oauth2.googleapis.com/token";

        public override string UserInfoUrl => "https://openidconnect.googleapis.com/v1/userinfo";

        public override string ClientId => settings.GoogleIdp.ClientId;

        public override string ClientSecret => settings.GoogleIdp.ClientSecret;

        public override Settings Settings => settings;

        public override IAuthHandler AuthHandler => authHandler;

        public override AuthTypes AuthType => AuthTypes.Microsoft;
    }
}
