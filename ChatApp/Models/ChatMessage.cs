using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models;

public class ChatMessage
{
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    [MaxLength(50)]
    public string? Sentiment { get; set; }

    // Link to user if authenticated, null if anonymous
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Display name for both authenticated and anonymous users
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public bool IsAnonymous => string.IsNullOrEmpty(UserId);
}