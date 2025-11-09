using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class ChatMessage
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    [MaxLength(50)]
    public string? Sentiment { get; set; }
}