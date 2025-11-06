using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs;
public class ChatHub : Hub
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ChatDbContext context, ILogger<ChatHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendMessage(string username, string message)
    {
        var chatMessage = new ChatMessage
        {
            Username = username,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Saved message {Id} from {User}", chatMessage.Id, username);

        await Clients.All.SendAsync("ReceiveMessage", new
        {
            id = chatMessage.Id,
            username = chatMessage.Username,
            message = chatMessage.Message,
            timestamp = chatMessage.Timestamp
        });
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
