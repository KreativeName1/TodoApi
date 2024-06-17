using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace TodoAPI.Controllers
{

  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class UserController : ControllerBase
  {
    private readonly ApplicationDbContext _connection;

    public UserController(ApplicationDbContext database)
    {
      _connection = database;
    }

    [HttpGet(Name = "GetUser")]
    public async Task<IActionResult> Get()
    {
      User user = await GetCurrentUser();
      if (user == null) return NotFound();
      return Ok(user);
    }
    private async Task<User> GetCurrentUser()
    {
      string userid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
      return _connection.Users.FirstOrDefault(u => u.Id == userid);
    }

    [HttpPut(Name = "UpdateUser")]
    public async Task<IActionResult> Put([FromBody] User user)
    {
      User existingUser = await GetCurrentUser();
      if (existingUser == null) return NotFound();

      existingUser.FirstName = user.FirstName;
      existingUser.LastName = user.LastName;
      await _connection.SaveChangesAsync();
      return Ok(existingUser);
    }

    [HttpDelete(Name = "DeleteUser")]
    public async Task<IActionResult> Delete()
    {
      User user = await GetCurrentUser();
      if (user == null) return NotFound();

      _connection.TodoNotes.RemoveRange(_connection.TodoNotes.Where(tn => tn.UserId == user.Id));
      _connection.Users.Remove(user);

      await _connection.SaveChangesAsync();
      return NoContent();
    }
  }
}