using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpGet("messages")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetMessages()
    {
        try
        {
            _logger.LogDebug("Fetching all chat messages");
            var messages = await _chatService.GetAllMessagesAsync();
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching messages");
            return StatusCode(500, new { error = "Failed to fetch messages" });
        }
    }

    [HttpGet("messages/{id}")]
    public async Task<ActionResult<ChatMessage>> GetMessage(int id)
    {
        try
        {
            var message = await _chatService.GetMessageByIdAsync(id);
            if (message == null)
            {
                _logger.LogWarning("Message with id {Id} not found", id);
                return NotFound(new { error = "Message not found" });
            }

            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching message {Id}", id);
            return StatusCode(500, new { error = "Failed to fetch message" });
        }
    }

    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        try
        {
            var deleted = await _chatService.DeleteMessageAsync(id);
            if (!deleted)
            {
                return NotFound(new { error = "Message not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {Id}", id);
            return StatusCode(500, new { error = "Failed to delete message" });
        }
    }
}