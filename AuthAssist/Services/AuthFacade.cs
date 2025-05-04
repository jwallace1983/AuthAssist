using AuthAssist.Services.Google;
using AuthAssist.Services.Local;
using AuthAssist.Services.Microsoft;
using AuthAssist.Services.OpenId;

namespace AuthAssist.Services
{
    public class AuthFacade(
        ILocalAuthService local,
        IGoogleAuthService google,
        IMicrosoftAuthService microsoft,
        Settings settings)
        : IAuthFacade
    {
        public Settings Settings => settings;

        public ILocalAuthService Local => local;

        public IOpenIdAuthService Google => google;

        public IOpenIdAuthService Microsoft => microsoft;
    }
}
