namespace ApiAuthDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValuesController(ILogger<ValuesController> logger) : ControllerBase
{
    /// <summary>
    /// API allows anonymous
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public IEnumerable<int> Get()
    {
        var rng = new Random();
        return Enumerable.Range(1, 3).Select(_ => rng.Next(0, 100));
    }

    /// <summary>
    /// API requires JWT auth
    /// </summary>
    /// <returns></returns>
    [HttpGet("jwt")]
    [Authorize]
    public IEnumerable<int> JwtAuth()
    {
        var username = User.Identity!.Name;
        logger.LogInformation("User [{username}] is visiting jwt auth", username);
        var rng = new Random();
        return Enumerable.Range(1, 10).Select(_ => rng.Next(0, 100));
    }

    /// <summary>
    /// API requires Basic auth
    /// </summary>
    /// <returns></returns>
    [HttpGet("basic")]
    [BasicAuth] // You can optionally provide a specific realm --> [BasicAuth("my-realm")]
    public IEnumerable<int> BasicAuth()
    {
        var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
        logger.LogInformation("basic auth from User [{username}]", username);
        var rng = new Random();
        return Enumerable.Range(1, 10).Select(_ => rng.Next(0, 100));
    }

    [HttpGet("basic-logout")]
    [BasicAuth]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult BasicAuthLogout()
    {
        logger.LogInformation("basic auth logout");
        // NOTE: there's no good way to log out basic authentication. This method is a hack.
        HttpContext.Response.Headers.WWWAuthenticate = "Basic realm=\"My Realm\"";
        return new UnauthorizedResult();
    }
}