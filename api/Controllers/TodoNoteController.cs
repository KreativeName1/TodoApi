using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TodoAPI.Controllers
{

  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class TodoNoteController : ControllerBase
  {
    private readonly ApplicationDbContext _connection;
    private readonly IAuthorizationService _authorizationService;

    public TodoNoteController(ApplicationDbContext database, IAuthorizationService authorizationService)
    {
      _connection = database;
      _authorizationService = authorizationService;
    }
    private async Task<User> GetCurrentUser()
    {
      string userid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
      return _connection.Users.FirstOrDefault(u => u.Id == userid);
    }


    [HttpGet("{id}", Name = "GetTodoNote")]
    public async Task<IActionResult> Get(int id)
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();
      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();
      return Ok(todoNote);
    }

    [HttpPost(Name = "CreateTodoNote")]
    public async Task<IActionResult> Post([FromBody] CreateModel todoNoteModel)
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      if (todoNoteModel == null) return BadRequest("Invalid client request");
      if (string.IsNullOrEmpty(todoNoteModel.Title) || string.IsNullOrEmpty(todoNoteModel.Content))
      {
        return BadRequest(new { message = "Title and Content are required" });
      }

      TodoNote todoNote = new TodoNote
      {
        Title = todoNoteModel.Title,
        Content = todoNoteModel.Content,
        DueDate = todoNoteModel.DueDate ?? null,
        IsComplete = false,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        UserId = user.Id
      };

      _connection.TodoNotes.Add(todoNote); _connection.SaveChanges();
      return CreatedAtRoute("GetTodoNote", new { id = todoNote.Id }, todoNote);
    }

    [HttpPut("{id}", Name = "UpdateTodoNote")]
    public async Task<IActionResult> Put(int id, [FromBody] TodoNote todoNote)
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();
      var existingTodoNote = await _connection.TodoNotes.FindAsync(id);
      if (existingTodoNote == null) return NotFound();

      var authorized = await _authorizationService.AuthorizeAsync(User, existingTodoNote, new TodoNoteAuthorizationRequirement(user.Id));
      if (!authorized.Succeeded) return Forbid();

      existingTodoNote.Title = todoNote.Title;
      existingTodoNote.Content = todoNote.Content;
      existingTodoNote.DueDate = todoNote.DueDate;
      existingTodoNote.IsComplete = todoNote.IsComplete;
      existingTodoNote.UpdatedAt = DateTime.Now;
      _connection.SaveChanges();
      return Ok(existingTodoNote);
    }

    [HttpDelete("{id}", Name = "DeleteTodoNote")]
    public async Task<IActionResult> Delete(int id)
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();

      _connection.TodoNotes.Remove(todoNote);
      _connection.SaveChanges();
      return Ok(todoNote);
    }


    [HttpGet("user/", Name = "GetTodoNotes")]
    public async Task<IActionResult> GetTodoNotes()
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNotes = await _connection.TodoNotes
        .Where(tn => tn.UserId == user.Id)
        .ToListAsync();

      return Ok(todoNotes);
    }
  // mark a todo note as complete
    [HttpPut("{id}/markComplete", Name = "MarkCompleteTodoNote")]
    public async Task<IActionResult> Complete(int id)
    {
      User user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();

      todoNote.IsComplete = true;
      _connection.SaveChanges();
      return Ok(todoNote);
    }
  }
}

public class CreateModel
{
  public string? Title { get; set; }
  public string? Content { get; set; }
  public DateTime? DueDate { get; set; }
}