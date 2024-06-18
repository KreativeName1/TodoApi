namespace TodoAPI.Models
{
  public class TodoNote
  {
    public int Id { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; } = null;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public TodoNote()
    {
    }

    public TodoNote(string title, string content, bool isComplete, DateTime createdAt, DateTime updatedAt, DateTime? dueDate)
    {
      Title = title;
      Content = content;
      IsComplete = isComplete;
      CreatedAt = createdAt;
      UpdatedAt = updatedAt;
      DueDate = dueDate;
    }
    public TodoNote(string title, string content, bool isComplete, DateTime? dueDate)
    {
      Title = title;
      Content = content;
      IsComplete = isComplete;
      DueDate = dueDate;
    }

    public override string ToString()
    {
      return $"TodoNote: {Title} {Content} {IsComplete} {CreatedAt} {UpdatedAt} {DueDate}";
    }
  }
}