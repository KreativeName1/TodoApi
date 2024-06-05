namespace TodoAPI.Models
{
  public class TodoNote
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
  }
}