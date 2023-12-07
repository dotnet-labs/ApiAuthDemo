using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiAuthDemo.Infrastructure.BasicAuth;

public class BasicAuthFilter : IAuthorizationFilter
{
    private readonly string _realm;

    public BasicAuthFilter(string realm)
    {
        _realm = realm;
        if (string.IsNullOrWhiteSpace(_realm))
        {
            throw new ArgumentNullException(nameof(realm), "Please provide a non-empty realm value.");
        }
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            var authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
            var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
            if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var credentials = Encoding.UTF8
                    .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                    .Split(':', 2);
                if (credentials.Length == 2)
                {
                    if (IsAuthorized(context, credentials[0], credentials[1]))
                    {
                        context.HttpContext.User.AddIdentity(new ClaimsIdentity(new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, credentials[0])
                        }));
                        return;
                    }
                }
            }

            ReturnUnauthorizedResult(context);
        }
        catch (FormatException)
        {
            ReturnUnauthorizedResult(context);
        }
    }
    public bool IsAuthorized(AuthorizationFilterContext context, string username, string password)
    {
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        return userService.IsValidUser(username, password);
    }

    private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
    {
        // Return 401 and a basic authentication challenge (causes browser to show login dialog)
        context.HttpContext.Response.Headers.WWWAuthenticate = $"Basic realm=\"{_realm}\"";
        context.Result = new UnauthorizedResult();
    }
}