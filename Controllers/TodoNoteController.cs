using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
namespace TodoAPI.Controllers
{

  [ApiController]
  [Route("[controller]")]
  public class TodoNoteController : ControllerBase
  {
    private readonly ApplicationDbContext _connection;

    public TodoNoteController(ApplicationDbContext database)
    {
      _connection = database;
    }

    [HttpGet(Name = "GetTodoNotes")]
    public IEnumerable<TodoNote> Get()
    {
      return _connection.TodoNotes;
    }

    [HttpGet("{id}", Name = "GetTodoNote")]
    public TodoNote Get(int id)
    {
      return _connection.TodoNotes.Find(id);
    }

    [HttpPost(Name = "CreateTodoNote")]
    public TodoNote Post([FromBody] TodoNote todoNote)
    {
      _connection.TodoNotes.Add(todoNote);
      _connection.SaveChanges();
      return todoNote;
    }

    [HttpPut("{id}", Name = "UpdateTodoNote")]
    public TodoNote Put(int id, [FromBody] TodoNote todoNote)
    {
      var existingTodoNote = _connection.TodoNotes.Find(id);
      existingTodoNote.Title = todoNote.Title;
      existingTodoNote.Content = todoNote.Content;
      existingTodoNote.DueDate = todoNote.DueDate;
      existingTodoNote.IsComplete = todoNote.IsComplete;
      _connection.SaveChanges();
      return existingTodoNote;
    }

    [HttpDelete("{id}", Name = "DeleteTodoNote")]
    public TodoNote Delete(int id)
    {
      var todoNote = _connection.TodoNotes.Find(id);
      _connection.TodoNotes.Remove(todoNote);
      _connection.SaveChanges();
      return todoNote;
    }


    [HttpGet("user/{userId}", Name = "GetTodoNotesByUser")]
    public IEnumerable<TodoNote> GetTodoNotesByUser(int userId)
    {
      return _connection.TodoNotes.Where(todoNote => todoNote.UserId == userId);
    }

    [HttpGet("user/{userId}/completed", Name = "GetCompletedTodoNotesByUser")]
    public IEnumerable<TodoNote> GetCompletedTodoNotesByUser(int userId)
    {
      return _connection.TodoNotes.Where(todoNote => todoNote.UserId == userId && todoNote.IsComplete);
    }

    [HttpGet("user/{userId}/incomplete", Name = "GetIncompleteTodoNotesByUser")]
    public IEnumerable<TodoNote> GetIncompleteTodoNotesByUser(int userId)
    {
      return _connection.TodoNotes.Where(todoNote => todoNote.UserId == userId && !todoNote.IsComplete);
    }
  }
}