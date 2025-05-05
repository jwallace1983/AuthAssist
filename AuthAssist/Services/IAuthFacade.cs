using AuthAssist.Services.Local;
using AuthAssist.Services.OpenId;

namespace AuthAssist.Services
{
    public interface IAuthFacade
    {
        ILocalAuthService Local { get; }

        IOpenIdAuthService Google { get; }
        
        IOpenIdAuthService Microsoft { get; }

        Settings Settings { get; }
    }
}