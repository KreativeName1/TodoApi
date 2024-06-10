namespace TodoAPI.Models
{
  public class TodoNote
  {
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public TodoNote()
    {
    }

    public TodoNote(string title, string content, bool isComplete, DateTime createdAt, DateTime updatedAt, DateTime? dueDate, int userId)
    {
      Title = title;
      Content = content;
      IsComplete = isComplete;
      CreatedAt = createdAt;
      UpdatedAt = updatedAt;
      DueDate = dueDate;
      UserId = userId;
    }
    public TodoNote(string title, string content, bool isComplete, DateTime? dueDate, int userId)
    {
      Title = title;
      Content = content;
      IsComplete = isComplete;
      DueDate = dueDate;
      UserId = userId;
    }

    public override string ToString()
    {
      return $"TodoNote: {Title} {Content} {IsComplete} {CreatedAt} {UpdatedAt} {DueDate} {UserId}";
    }
  }
}