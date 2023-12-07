namespace ApiAuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(ILogger<AccountController> logger) : ControllerBase
{
    /// <summary>
    /// JWT login
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userService"></param>
    /// <param name="tokenManagement"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request, [FromServices] IUserService userService,
        [FromServices] TokenManagement tokenManagement)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid Request");
        }

        if (!userService.IsValidUser(request.UserName, request.Password))
        {
            return BadRequest("Invalid Request");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name,request.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenManagement.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(
            tokenManagement.Issuer,
            tokenManagement.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(tokenManagement.AccessExpiration),
            signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        logger.LogInformation("User [{userName}] logged in the system.", request.UserName);
        return Ok(new LoginResult
        {
            UserName = request.UserName,
            JwtToken = token
        });
    }
}

public class LoginRequest
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>admin</example>
    [Required]
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    /// <example>securePassword</example>
    [Required]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>admin</example>
    public string UserName { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
}