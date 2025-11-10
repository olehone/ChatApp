using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async Task SendMessage(string username, string message)
    {
        try
        {
            var chatMessage = await _chatService.SaveMessageAsync(username, message);

            // Broadcast to all clients
            await Clients.All.SendAsync("ReceiveMessage", new
            {
                id = chatMessage.Id,
                username = chatMessage.Username,
                message = chatMessage.Message,
                timestamp = chatMessage.Timestamp,
                sentiment = chatMessage.Sentiment
            });

            _logger.LogDebug("Message broadcasted: {Id}", chatMessage.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendMessage");
            throw new InvalidOperationException(
            $"Failed to send message for user '{username}'. See inner exception for details.", ex);
        }
    }
}