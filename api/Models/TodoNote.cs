namespace TodoAPI.Models
{
  public class TodoNote
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsComplete { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public TodoNote()
    {
      Title = string.Empty;
      Content = string.Empty;
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