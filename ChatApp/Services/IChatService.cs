using ChatApp.Models;

namespace ChatApp.Services;

public interface IChatService
{
    Task<ChatMessage> SaveMessageAsync(string message, string displayName, string? userId);
    Task<IEnumerable<ChatMessage>> GetAllMessagesAsync();
    Task<ChatMessage?> GetMessageByIdAsync(int id);
    Task<bool> DeleteMessageAsync(int id);
}