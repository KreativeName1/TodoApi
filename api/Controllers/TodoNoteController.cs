using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
namespace TodoAPI.Controllers
{

  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class TodoNoteController : ControllerBase
  {
    private readonly ApplicationDbContext _connection;

    public TodoNoteController(ApplicationDbContext database)
    {
      _connection = database;
    }


    [HttpGet("{id}", Name = "GetTodoNote")]
    public TodoNote? Get(int id)
    {
      TodoNote? todoNote = _connection.TodoNotes.Find(id);
      if (todoNote == null) return null;
      return todoNote;
    }

    [HttpPost(Name = "CreateTodoNote")]
    public TodoNote Post([FromBody] TodoNote todoNote)
    {
      todoNote.CreatedAt = DateTime.Now;
      todoNote.UpdatedAt = DateTime.Now;
      _connection.TodoNotes.Add(todoNote);
      _connection.SaveChanges();
      return todoNote;
    }

    [HttpPut("{id}", Name = "UpdateTodoNote")]
    public TodoNote? Put(int id, [FromBody] TodoNote todoNote)
    {
      var existingTodoNote = _connection.TodoNotes.Find(id);
      if (existingTodoNote == null) return null;

      existingTodoNote.Title = todoNote.Title;
      existingTodoNote.Content = todoNote.Content;
      existingTodoNote.DueDate = todoNote.DueDate;
      existingTodoNote.IsComplete = todoNote.IsComplete;
      existingTodoNote.UpdatedAt = DateTime.Now;
      _connection.SaveChanges();
      return existingTodoNote;
    }

    [HttpDelete("{id}", Name = "DeleteTodoNote")]
    public TodoNote? Delete(int id)
    {
      TodoNote? todoNote = _connection.TodoNotes.Find(id);
      if (todoNote == null) return null;
      _connection.TodoNotes.Remove(todoNote);
      _connection.SaveChanges();
      return todoNote;
    }
  
    [HttpGet("user/", Name = "GetTodoNotes")]
    public IEnumerable<TodoNote> GetTodoNotes()
    {
      // get the todonotes that belong to the user that is authenticated
      return _connection.TodoNotes;

    }
  }
}