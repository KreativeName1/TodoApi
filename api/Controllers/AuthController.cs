using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TodoAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserManager<User> _userManager;
  private readonly SignInManager<User> _signInManager;
  private readonly IConfiguration _configuration;

 public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterModel model)
  {
    // check if the model is correct
    if (model == null) return BadRequest("Invalid client request");
    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
    {
      return BadRequest(new { message = "All fields are required" });
    }

    if (ModelState.IsValid)
    {
      // check if the user already exists
      var existingUser = await _userManager.FindByEmailAsync(model.Email);
      if (existingUser != null)
      {
        return BadRequest(new { message = "User already exists" });
      }
      // check if the password is correct
      if (model.Password.Length < 8)
      {
        return BadRequest(new { message = "Password must be at least 8 characters long" });
      }
      // create the user
      var user = new User(model.FirstName, model.LastName, model.Email)
      {
        UserName = model.Email,
        CreatedAt = DateTime.UtcNow
      };
      // save the user
      var result = await _userManager.CreateAsync(user, model.Password);
      if (result.Succeeded) return Ok();
      else return BadRequest(new { message = "User not created" });
    }

    return BadRequest(ModelState);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginModel model)
  {
    // check if the model is correct
    if (model == null) return BadRequest("Invalid client request");
    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
    {
      return BadRequest(new { message = "All fields are required" });
    }

    // check if the user exists and the password is correct
    User user = await _userManager.FindByEmailAsync(model.Email);
    if (user== null) return BadRequest(new { message = "Invalid email or password" });
    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
    if (result.Succeeded)
    {
      // generate token
      var token = GenerateToken(user);
      if (token == null) return Unauthorized();
      // return token
      return new ObjectResult(new
      {
        token = GenerateToken(user),
      });
    }
    return Unauthorized();
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    return Ok();
  }



  private JwtSecurityToken? GenerateToken(User user)
  {
    if (user == null) return null;
    if (user.Email == null) return null;
    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WmVog9i8K7V06StXfjueQOaRYfelo7N9A6TRe5rG1MIpFZRNyoL0E06aiSS9Sk10aeu3KdnLxkLHd5dXwEx5FpUARYf7kEXpSq9y"));
    var credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

    var authClaims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
    };

    var tokenDiscriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(authClaims),
      Expires = DateTime.UtcNow.AddMinutes(30),
      SigningCredentials = credentials
    };
    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.CreateToken(tokenDiscriptor);
    return (JwtSecurityToken)securityToken;
  }
}

public class RegisterModel
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; }
  public string? Password { get; set; }
}

public class LoginModel
{
  public string? Email { get; set; }
  public string? Password { get; set; }
}