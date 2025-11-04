using ChatApp.API.Data;
using ChatApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatDbContext _context;

    public ChatController(ChatDbContext context)
    {
        _context = context;
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessages()
        => await _context.ChatMessages.OrderByDescending(m => m.Timestamp).ToListAsync();

    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessage>> PostMessage(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
    }
}
