using Microsoft.AspNetCore.Identity;

namespace ChatApp.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}