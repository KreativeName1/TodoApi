using Microsoft.AspNetCore.Identity;

namespace TodoAPI.Models
{
  public class User : IdentityUser
  {

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }

    public DateTime? CreatedAt { get; set; }

    public List<TodoNote> TodoNotes { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public User()
    {
      CreatedAt = DateTime.Now;
    }


    public User(string firstName, string lastName, string password,string email)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
      Password = password;
    }

    public User(string firstName, string lastName, string email)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }
  }
}