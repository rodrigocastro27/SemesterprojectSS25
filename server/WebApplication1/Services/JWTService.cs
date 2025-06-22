using System;
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Models; 

namespace WebApplication1.Services
{
    
    public static class JwtTokenService
    {
        private static readonly string _secretKey = "thisisalongandverycomplexsecretkeyforjwttokenswhichshouldbeverysecuremorethan16characters"; // Minimum 16 characters for HS256, but ideally 32+ for strong security

        
        private static readonly string _issuer = "your_game_server.com";
        
        private static readonly string _audience = "your_flutter_app.com";
       
        private static readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(24);

   
        public static string GenerateToken(User user)
        {
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            
            
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

          
            var claims = new[]
            {
               
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()), 
               
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
               
                new Claim(ClaimTypes.Email, user.Email), 
                
                new Claim("userId", user.UserID.ToString()) 
                
            };

            
            var token = new JwtSecurityToken(
                issuer: _issuer,                   
                audience: _audience,                 
                claims: claims,                      
                expires: DateTime.UtcNow.Add(_tokenLifetime), 
                signingCredentials: credentials);   

            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      
    }
}
