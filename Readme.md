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

    public Task<AuthResult> AuthenticateUser(AuthRequest request)
    {
        // Perform app-specific authentication (including auto-disable, hashing, etc.)
        return _validUsers.Contains(request.Username, StringComparer.OrdinalIgnoreCase)
            ? Task.FromResult(new AuthResult { IsSuccess = true, Username = request.Username })
            : Task.FromResult(new AuthResult { IsSuccess = false });
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

## Customize available settings

Use the settings when configuring dependency injection to customize usage of the tool, such as:

* Change the endpoint for the api
* Enable non-https requests (not recommended)

For example, to modify the auth endpoint:

```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthAssist(settings =>
{
    settings.Endpoint = "/api/security/auth";
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
});
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
    }
}
```

If the response is not successful, the response will only contain the result.

```
{
    "IsSuccess": false
}
```

### Logout (/api/auth/logout)

A `POST` or `GET` request will log the user out by unsetting the HTTP-only cookie for the user.

```
GET: https://path-to-site/api/auth/logout
POST: https://path-to-site/api/auth/logout
```

## Configure authentication options

The API implements the following default behaviors for authentication options:

* Cookies have a sliding expiration of 20 minutes
* If an endpoint requires authentication, an empty response with `401` status code is provided.
* If an endpoint is authenticated but user does not meet authorization policies, an empty response with `403` status code is provided.
* Cookies are only set as HTTP-only cookies to avoid sharing any details with SPA applications beyond the response result.

Any of the cookie authentication options can be adjusted:

```
builder.Services.AddAuthAssist(settings =>
{
    settings.UseAuthHandler<AuthHandler>();
    settings.UseAuthPolicies(Policies.ApplyPolicies);
    settings.UseCookieOptions(options =>
    {
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = System.TimeSpan.FromMinutes(60);
    });
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
