using Microsoft.AspNetCore.Identity;

namespace TodoAPI.Models
{
  public class User : IdentityUser
  {

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public User()
    {
      CreatedAt = DateTime.Now;
      FirstName = string.Empty;
      LastName = string.Empty;
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