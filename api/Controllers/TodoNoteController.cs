using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
     private async Task<User?> GetCurrentUser()
    {
      User? user = await _connection.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
      if (user == null) return null;
      return user;
    }


    [HttpGet("{id}", Name = "GetTodoNote")]
    public async Task<IActionResult> Get(int id, [FromServices] IAuthorizationService authorizationService)
    {
      var user = await GetCurrentUser();
      if (user == null) return NotFound();

      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();

      var authorized = await _authorizationService.AuthorizeAsync(User, todoNote, new TodoNoteAuthorizationRequirement(user.Id));
      if (!authorized.Succeeded) return Forbid();

      return Ok(todoNote);
    }

    [HttpPost(Name = "CreateTodoNote")]
    public async Task<IActionResult> Post([FromBody] TodoNote todoNote)
    {
       var user = await GetCurrentUser();
      if (user == null) return Unauthorized();
      todoNote.CreatedAt = DateTime.Now;
      todoNote.UpdatedAt = DateTime.Now;
      todoNote.User = user;
      _connection.TodoNotes.Add(todoNote);_connection.SaveChanges();
      return CreatedAtRoute("GetTodoNote", new { id = todoNote.Id }, todoNote);
    }

    [HttpPut("{id}", Name = "UpdateTodoNote")]
    public async Task<IActionResult> Put(int id, [FromBody] TodoNote todoNote)
    {
      var user = await GetCurrentUser();
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

    // mark a todo note as complete
    [HttpPut("{id}/markComplete", Name = "MarkCompleteTodoNote")]
    public async Task<IActionResult> Complete(int id)
    {
       var user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();

      // Check authorization using authorization service
      var authorized = await _authorizationService.AuthorizeAsync(User, todoNote, new TodoNoteAuthorizationRequirement(user.Id));
      if (!authorized.Succeeded) return Forbid();

      todoNote.IsComplete = true;
      _connection.SaveChanges();
      return Ok(todoNote);
    }

    [HttpDelete("{id}", Name = "DeleteTodoNote")]
    public async Task<IActionResult> Delete(int id)
    {
      var user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNote = await _connection.TodoNotes.FindAsync(id);
      if (todoNote == null) return NotFound();

      // Check authorization using authorization service
      var authorized = await _authorizationService.AuthorizeAsync(User, todoNote, new TodoNoteAuthorizationRequirement(user.Id));
      if (!authorized.Succeeded) return Forbid();

      _connection.TodoNotes.Remove(todoNote);
      _connection.SaveChanges();
      return Ok(todoNote);
    }
    [HttpGet("user/", Name = "GetTodoNotes")]
    public async Task<IActionResult> GetTodoNotes()
    {
      var user = await GetCurrentUser();
      if (user == null) return Unauthorized();

      var todoNotes = await _connection.TodoNotes
        .Where(tn => tn.UserId == user.Id)
        .ToListAsync();

      return Ok(todoNotes);
    }
  }
}

public class TodoNoteAuthorizationRequirement : IAuthorizationRequirement
{
  public string UserId { get; }

  public TodoNoteAuthorizationRequirement(string userId)
  {
    UserId = userId;
  }
}
