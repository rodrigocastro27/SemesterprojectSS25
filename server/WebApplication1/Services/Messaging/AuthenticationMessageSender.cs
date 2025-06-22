using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models; 

namespace WebApplication1.Services.Messaging
{
  
    public static class AuthMessageSender
    {
     
        public static async Task SendSuccess(WebSocket socket, string action, string message, object? additionalData = null)
        {
            var response = new
            {
                type = $"{action}_response",
                status = "success",
                message = message,
                data = additionalData
            };
            await SendJsonMessage(socket, response);
        }

       
        public static async Task SendError(WebSocket socket, string action, string errorMessage, int? errorCode = null)
        {
            var response = new
            {
                type = $"{action}_response",
                status = "error",
                message = errorMessage,
                errorCode = errorCode
            };
            await SendJsonMessage(socket, response);
        }

        
        public static async Task SendRegistrationSuccess(WebSocket socket, int userId, string email)
        {
            var data = new { userId = userId, email = email };
            await SendSuccess(socket, "register", "User registered successfully.", data);
        }

       
        public static async Task SendRegistrationFailure(WebSocket socket, string reason)
        {
            await SendError(socket, "register", reason);
        }

        
        public static async Task SendLoginSuccess(WebSocket socket, User user, string token)
        {
            var data = new { userId = user.UserID, email = user.Email, token = token };
            await SendSuccess(socket, "login", "Login successful.", data);
        }

     
        public static async Task SendLoginFailure(WebSocket socket, string reason)
        {
            await SendError(socket, "login", reason);
        }

  
        public static async Task SendPasswordResetRequestSuccess(WebSocket socket, string email)
        {
            
            await SendSuccess(socket, "password_reset_request", $"If '{email}' is registered, a password reset link has been sent.");
        }

       
        public static async Task SendPasswordResetRequestFailure(WebSocket socket, string reason)
        {
            await SendError(socket, "password_reset_request", reason);
        }

        
        public static async Task SendPasswordUpdateSuccess(WebSocket socket)
        {
            await SendSuccess(socket, "password_update", "Password updated successfully.");
        }

      
        public static async Task SendPasswordUpdateFailure(WebSocket socket, string reason)
        {
            await SendError(socket, "password_update", reason);
        }

        
        private static async Task SendJsonMessage(WebSocket socket, object messageObject)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(messageObject);
                var bytes = Encoding.UTF8.GetBytes(jsonMessage);
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error sending message: {ex.Message}");
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing or sending JSON message: {ex.Message}");
            }
        }
    }
}
