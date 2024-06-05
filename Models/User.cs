using Microsoft.AspNetCore.Identity;

namespace TodoAPI.Models
{
  public class User : IdentityUser
  {

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }

    public string Email { get; set; }

    public List<TodoNote> TodoNotes { get; set; }


    public User()
    {
    }

    public User(string firstName, string lastName, string email)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    public User(string firstName, string lastName, string email, string password)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
      Password = password;
    }
  }
}