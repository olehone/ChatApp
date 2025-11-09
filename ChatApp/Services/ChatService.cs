using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services;

public class ChatService : IChatService
{
    private readonly ChatDbContext _context;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        ChatDbContext context,
        ISentimentAnalysisService sentimentService,
        ILogger<ChatService> logger)
    {
        _context = context;
        _sentimentService = sentimentService;
        _logger = logger;
    }

    public async Task<ChatMessage> SaveMessageAsync(string message, string displayName, string? userId)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be empty", nameof(message));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));
        }

        var chatMessage = new ChatMessage
        {
            Message = message,
            DisplayName = displayName,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        // Analyze sentiment
        try
        {
            var sentiment = await _sentimentService.AnalyzeSentimentAsync(message);
            chatMessage.Sentiment = sentiment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze sentiment, continuing without it");
        }

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Message saved: Id={Id}, User={DisplayName}, IsAnonymous={IsAnon}, Sentiment={Sentiment}",
            chatMessage.Id, displayName, chatMessage.IsAnonymous, chatMessage.Sentiment ?? "N/A");

        return chatMessage;
    }

    public async Task<IEnumerable<ChatMessage>> GetAllMessagesAsync()
    {
        return await _context.ChatMessages
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<ChatMessage?> GetMessageByIdAsync(int id)
    {
        return await _context.ChatMessages.FindAsync(id);
    }

    public async Task<bool> DeleteMessageAsync(int id)
    {
        var message = await _context.ChatMessages.FindAsync(id);
        if (message == null)
        {
            _logger.LogWarning("Attempted to delete non-existent message with Id={Id}", id);
            return false;
        }

        _context.ChatMessages.Remove(message);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Message deleted: Id={Id}", id);
        return true;
    }
}