using WebApplication1.Utils; 
using WebApplication1.Services; 
using WebApplication1.Services.Messaging; 

namespace WebApplication1.Handlers
{
   
    public static class AuthHandlers
    {
   
        public static void Register(WebSocketActionDispatcher dispatcher)
        {
            
            dispatcher.Register("register_request", async (data, socket) =>
            {
                if (data.TryGetProperty("email", out var emailElement) &&
                    data.TryGetProperty("password", out var passwordElement))
                {
                    string? email = emailElement.GetString();
                    string? password = passwordElement.GetString();
                    string? googleId = null;

                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("[register_request] Invalid email or password received.");
                        await AuthMessageSender.SendRegistrationFailure(socket, "Email and password cannot be empty.");
                        return;
                    }

                    Console.WriteLine($"\n[register_request] Attempting to register user: {email}");

             
                    var newUser = await AuthenticationManager.Instance.RegisterUser(email, password, googleId);

                    if (newUser != null)
                    {
                        Console.WriteLine($"Registration successful for {email}. UserID: {newUser.UserID}");
                        
                        await AuthMessageSender.SendRegistrationSuccess(socket, newUser.UserID, newUser.Email);
                    }
                    else
                    {
                        Console.WriteLine($"Registration failed for {email}.");
                        await AuthMessageSender.SendRegistrationFailure(socket, "Registration failed. Email might already be in use.");
                    }
                }
                else
                {
                    Console.WriteLine("[register_request] Missing 'email' or 'password' in request data.");
                    await AuthMessageSender.SendRegistrationFailure(socket, "Invalid request format.");
                }
            });

            
            dispatcher.Register("login_request", async (data, socket) =>
            {
                if (data.TryGetProperty("email", out var emailElement) &&
                    data.TryGetProperty("password", out var passwordElement))
                {
                    string? email = emailElement.GetString();
                    string? password = passwordElement.GetString();

                    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                    {
                        Console.WriteLine("[login_request] Invalid email or password received.");
                        await AuthMessageSender.SendLoginFailure(socket, "Email and password cannot be empty.");
                        return;
                    }

                    Console.WriteLine($"\n[login_request] Attempting to login user: {email}");

                 
                    var authenticatedUser = await AuthenticationManager.Instance.AuthenticateUser(email, password);

                    if (authenticatedUser != null)
                    {
                        Console.WriteLine($"Login successful for {email}. UserID: {authenticatedUser.UserID}");
                        
                       
                        string jwtToken = JwtTokenService.GenerateToken(authenticatedUser);

                        
                        await AuthMessageSender.SendLoginSuccess(socket, authenticatedUser, jwtToken);

                       
                        
                    }
                    else
                    {
                        Console.WriteLine($"Login failed for {email}.");
                        await AuthMessageSender.SendLoginFailure(socket, "Invalid email or password.");
                    }
                }
                else
                {
                    Console.WriteLine("[login_request] Missing 'email' or 'password' in request data.");
                    await AuthMessageSender.SendLoginFailure(socket, "Invalid request format.");
                }
            });

       
            dispatcher.Register("password_reset_request", async (data, socket) =>
            {
                if (data.TryGetProperty("email", out var emailElement))
                {
                    string? email = emailElement.GetString();

                    if (string.IsNullOrEmpty(email))
                    {
                        Console.WriteLine("[password_reset_request] Email cannot be empty.");
                        await AuthMessageSender.SendPasswordResetRequestFailure(socket, "Email cannot be empty.");
                        return;
                    }

                    Console.WriteLine($"\n[password_reset_request] Attempting to initiate password reset for: {email}");

                  
                    var resetTokenUser = await AuthenticationManager.Instance.CreatePasswordResetToken(email);

                    
                    if (resetTokenUser != null && !string.IsNullOrEmpty(resetTokenUser.Token))
                    {
                        Console.WriteLine($"Generated password reset token for {email}: {resetTokenUser.Token}. This token should be EMAILED to the user.");
                        await AuthMessageSender.SendPasswordResetRequestSuccess(socket, email);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to generate password reset token for {email}.");
                        
                        await AuthMessageSender.SendPasswordResetRequestSuccess(socket, email);
                    }
                }
                else
                {
                    Console.WriteLine("[password_reset_request] Missing 'email' in request data.");
                    await AuthMessageSender.SendPasswordResetRequestFailure(socket, "Invalid request format.");
                }
            });

           
            dispatcher.Register("password_update", async (data, socket) =>
            {
                if (data.TryGetProperty("token", out var tokenElement) &&
                    data.TryGetProperty("newPassword", out var newPasswordElement))
                {
                    string? token = tokenElement.GetString();
                    string? newPassword = newPasswordElement.GetString();

                    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                    {
                        Console.WriteLine("[password_update] Invalid token or new password received.");
                        await AuthMessageSender.SendPasswordUpdateFailure(socket, "Token and new password cannot be empty.");
                        return;
                    }

                    Console.WriteLine($"\n[password_update] Attempting to update password with token: {token}");

                    // Delegate logic to AuthenticationManager
                    bool success = await AuthenticationManager.Instance.ResetPassword(token, newPassword);

                    if (success)
                    {
                        Console.WriteLine($"Password updated successfully with token: {token}");
                        await AuthMessageSender.SendPasswordUpdateSuccess(socket);
                    }
                    else
                    {
                        Console.WriteLine($"Password update failed with token: {token}");
                        await AuthMessageSender.SendPasswordUpdateFailure(socket, "Password update failed. Token might be invalid or expired.");
                    }
                }
                else
                {
                    Console.WriteLine("[password_update] Missing 'token' or 'newPassword' in request data.");
                    await AuthMessageSender.SendPasswordUpdateFailure(socket, "Invalid request format.");
                }
            });
        }
    }
}
