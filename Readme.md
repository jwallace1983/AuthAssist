# AuthAssist

"AuthAssist" is an open-source project to enable quick configuration of Microsoft Identity libraries for APIs.
This is intended as a lightweight, opinionated solution to add extensible login/logout features to
small and medium scale APIs.

With only a few lines of code, quickly focus on the application-specific authentication and authorization. Successful
authentication will ensure http-only cookies are encrypted and managed for secure APIs, especially for SPA applications.

## Enable AuthAssist

1. Implement `ApplyPolicies(...)` method to configure how policies are applied to the application.

```
public static class Policies
{
    public const string ADMIN = "admin";

    public static void ApplyPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(ADMIN, builder => builder.RequireClaim(ADMIN));
    }
}
```

2. Implement `IAuthHandler` to define how a user is authenticated and which claims to when authentication succeeds.

```
public class AuthHandler : IAuthHandler
{
    private static readonly string[] _validUsers = new string[] { "user", "admin" };

    public Task AppendClaims(string username, List<Claim> claims)
    {
        // Add claims to the provided list
        return Task.CompletedTask;
    }

    public Task<bool> VerifyUser(ClaimsPrincipal user) => Task.FromResult(true);

    public Task<AuthResult> AuthenticateUser(AuthRequest request)
    {
        return _validUsers.Contains(request.Username, StringComparer.OrdinalIgnoreCase)
            ? Task.FromResult(new AuthResult { Username = request.Username })
            : Task.FromResult(new AuthResult { Error = "auth.failed" });
    }
}
```

3. Configure the dependency injection by calling `AddAuthAssist(...)` extension method when configuring the services.

```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
});
builder.Services.AddControllers();
```

4. Enable the middleware by calling `UseAuthAssist()` extension method on the application object.

```
var app = builder.Build();
app.UseAuthAssist();
app.MapControllers();
app.Run();
```

5. Apply `Authorize` or custom security attributes to API endpoints.

```
[HttpGet("public")]
public string GetPublic() => "Public endpoint";

[HttpGet("secure"), Authorize]
public string GetSecure() => "Secure endpoint: " + User.Identity.Name;

[HttpGet("admin"), Authorize(Policies.ADMIN)]
public string GetClaim() => "Admin endpoint: " + User.Identity.Name;
```

## API Endpoints

The following sample API endpoints are supported by the auth api.

### Login (/api/auth/login)

A `POST` request should be used to authenticate.

```
POST: https://path-to-site/api/auth/login
{
    "username": "user",
    "password": "welcome123"
}
```

The response will contain the results of the authentication. If successful, an HTTP-only cookie will be set for the user.

```
{
    "IsSuccess": true,
    "Username": "user",
    "Claims": {
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "user"
    },
    "ExpiresUtc": "2022-08-25T04:50:48.9066792Z"
}
```

If the response is not successful, the response will only contain the result and a provided error code.

```
{
    "IsSuccess": false,
    "Error": "auth.invalid"
}
```

### Extend (/api/auth/extend)

A `GET` request may be used to extend the token.

```
GET: https://path-to-site/api/auth/extend
```

The response will contain the results of the authentication. If successful, an HTTP-only cookie will be set for the user.

```
{
    "IsSuccess": true,
    "Username": "user",
    "Claims": {
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "user"
    },
    "ExpiresUtc": "2022-08-25T04:50:48.9066792Z"
}
```

If the response is not successful, the response will only contain the result and a provided error code.

```
{
    "IsSuccess": false,
    "Error": "auth.invalid"
}
```

### Logout (/api/auth/logout)

A `GET` request will log the user out by unsetting the HTTP-only cookie for the user.

```
GET: https://path-to-site/api/auth/logout
```

### Token (/api/auth/token)

A `GET` request will return the current token for the user, if the endpoint is enabled.

```
GET: https://path-to-site/api/auth/token
```

### Google (/api/auth/google)

A redirection to the endpoint will redirect the user to the Google login page for social login.

### Google Callback (/api/auth/google/callback)

The user will be returned to the Google authentication callback when a user complets the social login flow.

### Microsoft (/api/auth/microsoft)

A redirection to the endpoint will redirect the user to the Microsoft login page for social login.

### Microsoft Callback (/api/auth/microsoft/callback)

The user will be returned to the Microsoft authentication callback when a user complets the social login flow.


## Configure authentication options

The API implements the following default behaviors for authentication options:

* Cookies have a default expiration of 20 minutes
* If an endpoint requires authentication, an empty response with `401` status code is provided.
* If an endpoint is authenticated but user does not meet authorization policies, an empty response with `403` status code is provided.
* Cookies are only set as HTTP-only cookies to avoid sharing any details with SPA applications beyond the response result.

Add social authentication (`.Google` or `.Microsoft`):

```
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.Google.ClientId = builder.Configuration["SocialAuth:Google:ClientId"];
    settings.Google.ClientSecret = builder.Configuration["SocialAuth:Google:ClientSecret"];
    settings.Microsoft.ClientId = builder.Configuration["SocialAuth:Microsoft:ClientId"];
    settings.Microsoft.ClientSecret = builder.Configuration["SocialAuth:Microsoft:ClientSecret"];
});
```

Any of the cookie authentication options can be adjusted:

```
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.UseCookieOptions(options =>
    {
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
    });
    settings.JsonSerializerOptions; // Customize any json serialization options
});
```

Also, the cookie policy can also be customized:

```
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.CookiePolicyOptions = new CookiePolicyOptions
    {
        Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always,
        HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    };
});
```

Additional configuration options from the `Settings` include:

* `Prefix`: The prefix for the authentication API endpoints. Default is `/api/auth`
* `RedirectToLogin` : Redirect to a login page instead of displaying a 401 error. Default to show 401 error.
* `RedirectToAccessDenied`: Redirect to an access denied page instead of a 403 error. Default to show 403 error.
* `RedirectToLoginError`: Redirect to a URL when a social login is not successful.
* `DefaultReturnUrl`: Default return url for social login, when a return url is not provided on initial request.
* `RequireHttps`: Require HTTPS for the authentication API. Default is true.
* `EnableTokenEndpoint`: Enable the token endpoint to return the current token. Default is false.
* `JsonSerializerOptions`: Customize the JSON serialization options for the API responses.

## Error Codes

When an error occurs, a response AuthResult object is returned, such as

```
{
    "IsSuccess": false,
    "Error": "user.invalid"
}
```

The following error codes may be returned:

* user.invalid: User not valid or able to be loaded (ex: unauthenticated)
* auth.code.invalid: The authentication code from social login is not valid
* auth.nonce.invalid: Unable to validate state/nonce from social login
* auth.nonce.mismatch: The social state/nonce from social login does not match generated value
* auth.token.invalid: Unable to exchange authenticate code for access token
* auth.userinfo.invalid: Unable to load authenticated user information from social login
