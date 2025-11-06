using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatDbContext context, ILogger<ChatController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessages()
    {
        _logger.LogInformation("Fetching all chat messages...");
        var messages = await _context.ChatMessages.OrderByDescending(m => m.Timestamp).ToListAsync();
        return Ok(messages);
    }

    [HttpGet("messages/{id}")]
    public async Task<ActionResult<ChatMessage>> GetMessage(int id)
    {
        var message = await _context.ChatMessages.FindAsync(id);
        if (message == null)
        {
            _logger.LogWarning("Message with id {Id} not found.", id);
            return NotFound();
        }
        return Ok(message);
    }

    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessage>> CreateMessage(ChatMessage msg)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid message input received.");
            return BadRequest(ModelState);
        }

        try
        {
            msg.Timestamp = DateTime.UtcNow;
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Message created by {User}.", msg.Username);

            return CreatedAtAction(nameof(GetMessage), new { id = msg.Id }, msg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating message.");
            return StatusCode(500, "An error occurred while creating the message.");
        }
    }


    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var msg = await _context.ChatMessages.FindAsync(id);
        if (msg == null)
        {
            _logger.LogWarning("Attempt to delete non-existent message with id {Id}.", id);
            return NotFound();
        }

        _context.ChatMessages.Remove(msg);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Message with id {Id} deleted.", id);

        return NoContent();
    }
}
