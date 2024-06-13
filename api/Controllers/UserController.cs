using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
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
      var user = await _connection.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
      if (user == null) return NotFound();
      return Ok(user); // Return only necessary user information
    }

    [HttpPut(Name = "UpdateUser")]
    public async Task<IActionResult> Put([FromBody] User user)
    {
      var existingUser = await _connection.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
      if (existingUser == null) return NotFound();

      existingUser.FirstName = user.FirstName;
      existingUser.LastName = user.LastName;
      await _connection.SaveChangesAsync();
      return Ok(existingUser); // Return only necessary user information
    }

    [HttpDelete(Name = "DeleteUser")]
    public async Task<IActionResult> Delete()
    {
      var user = await _connection.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
      if (user == null) return NotFound();

      //delete all todo notes associated with the user
      _connection.TodoNotes.RemoveRange(_connection.TodoNotes.Where(tn => tn.UserId == user.Id));
      _connection.Users.Remove(user);

      await _connection.SaveChangesAsync();
      return NoContent(); // Don't return user data on delete
    }
  }
}