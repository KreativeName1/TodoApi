
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI
{
  public class ApplicationDbContext : DbContext
  {
    public DbSet<User> Users { get; set; }

    public DbSet<TodoNote> TodoNotes { get; set; }
  }
}