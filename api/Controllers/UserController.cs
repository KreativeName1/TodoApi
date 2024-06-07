using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using BCrypt.Net;
namespace TodoAPI.Controllers
{

  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly ApplicationDbContext _connection;

    public UserController(ApplicationDbContext database)
    {
      _connection = database;
    }

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
      return _connection.Users;
    }

    [HttpGet("{id}", Name = "GetUser")]
    public User Get(int id)
    {
      return _connection.Users.Find(id);
    }

    // get with email
    [HttpGet("email/{email}", Name = "GetUserByEmail")]
    public IActionResult Get(string email)
    {
      var user = _connection.Users.FirstOrDefault(u => u.Email == email);

      if (user == null)
      {
        return NotFound(new { message = "User not found" });
      }
      Console.WriteLine(user);
      return Ok(user);
    }


    [HttpPost(Name = "CreateUser")]
    public User Post([FromBody] User user)
    {
      user.CreatedAt = DateTime.Now;
      _connection.Users.Add(user);
      _connection.SaveChanges();
      return user;
    }

    [HttpPut("{id}", Name = "UpdateUser")]
    public User Put(int id, [FromBody] User user)
    {
      var existingUser = _connection.Users.Find(id);
      existingUser.FirstName = user.FirstName;
      existingUser.LastName = user.LastName;
      existingUser.Email = user.Email;
      existingUser.Password = user.Password;
      _connection.SaveChanges();
      return existingUser;
    }

    [HttpDelete("{id}", Name = "DeleteUser")]
    public User Delete(int id)
    {
      var user = _connection.Users.Find(id);
      _connection.Users.Remove(user);
      _connection.SaveChanges();
      return user;
    }
  }
}