using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TodoAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly UserManager<User> _userManager;
  private readonly SignInManager<User> _signInManager;

  public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
  {
    _userManager = userManager;
    _signInManager = signInManager;
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterModel model)
  {
    if (ModelState.IsValid)
    {
      var existingUser = await _userManager.FindByEmailAsync(model.Email);
      if (existingUser != null)
      {
        return BadRequest(new { message = "User already exists" });
      }
      if (model.Password.Length < 8)
      {
        return BadRequest(new { message = "Password must be at least 8 characters long" });
      }

      var user = new User(model.FirstName, model.LastName, model.Email)
      {
        UserName = model.Email,
        CreatedAt = DateTime.UtcNow
      };

      var result = await _userManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        return Ok();
      }
      else
      {
        return BadRequest(result.Errors);
      }
    }

    return BadRequest(ModelState);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginModel model)
  {
    if (ModelState.IsValid)
    {
      var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        HttpContext.Session.SetString("UserEmail", model.Email);
        return Ok();
      }
      else
      {
        return Unauthorized();
      }
    }

    return BadRequest(ModelState);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    HttpContext.Session.Remove("UserEmail");
    return Ok();
  }
}



public class RegisterModel
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
}

public class LoginModel
{
  public string Email { get; set; }
  public string Password { get; set; }
}