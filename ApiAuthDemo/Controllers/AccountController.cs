using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiAuthDemo.Infrastructure.Jwt;
using ApiAuthDemo.Services;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuthDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    private readonly TokenManagement _tokenManagement;
    public AccountController(ILogger<AccountController> logger, IUserService userService, TokenManagement tokenManagement)
    {
        _logger = logger;
        _userService = userService;
        _tokenManagement = tokenManagement;
    }

    /// <summary>
    /// JWT login
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid Request");
        }

        if (!_userService.IsValidUser(request.UserName, request.Password))
        {
            return BadRequest("Invalid Request");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name,request.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(
            _tokenManagement.Issuer,
            _tokenManagement.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
            signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        _logger.LogInformation("User [{userName}] logged in the system.", request.UserName);
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