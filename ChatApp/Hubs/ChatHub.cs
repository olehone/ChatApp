using Azure;
using Azure.AI.TextAnalytics;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs;

public class ChatHub : Hub
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChatHub> _logger;
    private readonly TextAnalyticsClient _textClient;

    public ChatHub(ChatDbContext context, ILogger<ChatHub> logger, IConfiguration config)
    {
        _context = context;
        _logger = logger;

        var endpoint = config["AzureAI:TextAnalyticsEndpoint"]
            ?? throw new InvalidOperationException("Missing AzureAI:TextAnalyticsEndpoint configuration.");

        var key = config["AzureAI:TextAnalyticsKey"]
            ?? throw new InvalidOperationException("Missing AzureAI:TextAnalyticsKey configuration.");

        _textClient = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
    }

    public async Task SendMessage(string username, string message)
    {
        var chatMessage = new ChatMessage
        {
            Username = username,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        var sentimentResult = await _textClient.AnalyzeSentimentAsync(message);
        var sentiment = sentimentResult.Value.Sentiment.ToString();

        chatMessage.Sentiment = sentiment;

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Saved message {Id} from {User} with sentiment {Sentiment}",
            chatMessage.Id, username, sentiment);

        await Clients.All.SendAsync("ReceiveMessage", new
        {
            id = chatMessage.Id,
            username = chatMessage.Username,
            message = chatMessage.Message,
            timestamp = chatMessage.Timestamp,
            sentiment
        });
    }
}
