using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
      return _connection.Users;
    }

    [HttpGet("{id}", Name = "GetUser")]
    public User? Get(int id)
    {
      User? user = _connection.Users.Find(id);
      if (user == null) return null;
      return _connection.Users.Find(id);
    }

    [HttpPut("{id}", Name = "UpdateUser")]
    public User? Put(int id, [FromBody] User user)
    {
      var existingUser = _connection.Users.Find(id);
      if (existingUser == null) return null;
      existingUser.FirstName = user.FirstName;
      existingUser.LastName = user.LastName;
      existingUser.Email = user.Email;
      existingUser.Password = user.Password;
      _connection.SaveChanges();
      return existingUser;
    }

    [HttpDelete("{id}", Name = "DeleteUser")]
    public User? Delete(int id)
    {
      User? user = _connection.Users.Find(id);
      if (user == null) return null;
      _connection.Users.Remove(user);
      _connection.SaveChanges();
      return user;
    }
  }
}