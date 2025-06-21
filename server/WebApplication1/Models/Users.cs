using Newtonsoft.Json.Schema;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Messaging;
using WebApplication1.Utils;

namespace WebApplication1.Models;

public class User (int userId, string email, string passwordHash, string? googleId = null)
{
    public int UserId { get; set; } = userId;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public string? GoogleId { get; set; } = googleId;
    
    public int TokenID { get; set; } 
    public string Token { get; set; } 
    public DateTime ExpiresAt { get; set; } 
    public bool Used { get; set; }
    
    public bool IsValid()
    {
        return !Used && ExpiresAt > DateTime.Now;
    }
    
        
}

