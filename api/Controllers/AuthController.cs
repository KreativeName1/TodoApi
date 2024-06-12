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
  private readonly IConfiguration _configuration;

  public AuthController(UserManager<User> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
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
      else return BadRequest(result.Errors);
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
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
    {
      // generate token
      var token = GenerateToken(user);
      var refreshToken = GenerateRefreshToken();

      // Save refresh token with the user
      user.RefreshToken = refreshToken;
      user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Set refresh token expiry time
      await _userManager.UpdateAsync(user);
      if (token == null) return Unauthorized();
      // return token
      return Ok(new
      {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expiration = token.ValidTo,
        refreshToken = refreshToken
      });
    }
    return Unauthorized();
  }

  [HttpPost("refresh-token")]
  public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
  {
    // check if the model is correct
    if (tokenModel is null) return BadRequest("Invalid client request");

    if (string.IsNullOrEmpty(tokenModel.AccessToken) || string.IsNullOrEmpty(tokenModel.RefreshToken))
    {
      return BadRequest(new { message = "All fields are required" });
    }

    // get the access token and refresh token
    string accessToken = tokenModel.AccessToken;
    string refreshToken = tokenModel.RefreshToken;


    // get the principal from the expired token
    var principal = GetPrincipalFromExpiredToken(accessToken);
    if (principal == null)
    {
      return BadRequest("Invalid access token or refresh token");
    }
    if (principal.Identity == null)
    {
      return BadRequest("Invalid access token or refresh token");
    }

    // get the email from the principal
    var email = principal.Identity.Name;
    if (string.IsNullOrEmpty(email))
    {
      return BadRequest("Invalid access token or refresh token");
    }

    // get the user from the email
    var user = await _userManager.FindByEmailAsync(email);

    // check if the user exists and the refresh token is correct
    if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
    {
      return BadRequest("Invalid access token or refresh token");
    }

    // generate new token
    var newAccessToken = GenerateToken(user);
    var newRefreshToken = GenerateRefreshToken();

    // Save refresh token with the user
    user.RefreshToken = newRefreshToken;
    await _userManager.UpdateAsync(user);

    if (newAccessToken == null) return Unauthorized();
    // return token
    return new ObjectResult(new
    {
      token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
      expiration = newAccessToken.ValidTo,
      refreshToken = newRefreshToken
    });
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    var user = await _userManager.GetUserAsync(User);
    if (user != null)
    {
      user.RefreshToken = null;
      user.RefreshTokenExpiryTime = DateTime.Now;
      await _userManager.UpdateAsync(user);
    }
    return Ok();
  }



  private JwtSecurityToken? GenerateToken(User user)
  {
    if (user == null) return null;
    if (user.Email == null) return null;
    var authClaims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Email),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

    var token = new JwtSecurityToken(
      issuer: _configuration["JWT:ValidIssuer"],
      audience: _configuration["JWT:ValidAudience"],
      expires: DateTime.Now.AddMinutes(15),
      claims: authClaims,
      signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return token;
  }

  private string GenerateRefreshToken()
  {
    var randomNumber = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }
  }

  private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
  {
    var tokenValidationParameters = new TokenValidationParameters
    {
      ValidateAudience = false,
      ValidateIssuer = false,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
      ValidateLifetime = false
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    SecurityToken securityToken;
    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
    var jwtSecurityToken = securityToken as JwtSecurityToken;
    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
    {
      throw new SecurityTokenException("Invalid token");
    }

    return principal;
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

public class TokenModel
{
  public string? AccessToken { get; set; }
  public string? RefreshToken { get; set; }
}