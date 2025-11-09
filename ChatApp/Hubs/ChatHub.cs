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

    public async Task SendMessage(string displayName, string message)
    {
        try
        {
            // Get userId if authenticated, null if anonymous
            var userId = Context.User?.Identity?.IsAuthenticated == true
                ? Context.UserIdentifier
                : null;

            // Save message with sentiment analysis
            var chatMessage = await _chatService.SaveMessageAsync(message, displayName, userId);

            // Broadcast to all clients
            await Clients.All.SendAsync("ReceiveMessage", new
            {
                id = chatMessage.Id,
                displayName = chatMessage.DisplayName,
                message = chatMessage.Message,
                timestamp = chatMessage.Timestamp,
                sentiment = chatMessage.Sentiment,
                isAnonymous = chatMessage.IsAnonymous
            });

            _logger.LogDebug("Message broadcasted: {Id}", chatMessage.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendMessage");
            throw;
        }
    }
}