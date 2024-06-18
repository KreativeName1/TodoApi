using Microsoft.AspNetCore.Identity;

namespace TodoAPI.Models
{
  public class User : IdentityUser
  {

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public User()
    {
    }

    public User(string firstName, string lastName, string email)
    {
      FirstName = firstName;
      LastName = lastName;
      Email = email;
    }

    public override string ToString()
    {
      return $"User: {FirstName} {LastName} {Email} {CreatedAt}";
    }
  }
}