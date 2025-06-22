using WebApplication1.Models; 
using WebApplication1.Database; 

using WebApplication1.Data; 

namespace WebApplication1.Services
{
    
    public class AuthenticationManager
    {
       
        public static AuthenticationManager Instance { get; } = new AuthenticationManager();

      
        private AuthenticationManager() { }

      
        private static string HashPassword(string password)
        {
            Console.WriteLine("WARNING: Using a placeholder password hashing method. Implement BCrypt.Net-Next or Argon2 for production.");
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            Console.WriteLine("WARNING: Using a placeholder password verification method. Implement BCrypt.Net-Next or Argon2 for production.");
            return HashPassword(password) == hashedPassword;
        }

     
        public async Task<User?> RegisterUser(string email, string password, string? googleId = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Registration failed: Email or password cannot be empty.");
                return null;
            }

           
            if (DatabaseHandler.Instance.SelectUserByEmail(email) != null)
            {
                Console.WriteLine($"Registration failed: Email '{email}' already exists.");
                return null;
            }

            string hashedPassword = HashPassword(password);

            try
            {
                
                int newUserId = DatabaseHandler.Instance.InsertUser(email, hashedPassword, googleId);
                Console.WriteLine($"User registered: {email} with UserID: {newUserId}");
                return new User(newUserId, email, hashedPassword, googleId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user registration: {ex.Message}");
                return null;
            }
        }

        
        public async Task<User?> AuthenticateUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Authentication failed: Email or password cannot be empty.");
                return null;
            }

            try
            {
                
                var user = DatabaseHandler.Instance.SelectUserByEmail(email);

                if (user != null)
                {
                    
                    if (VerifyPassword(password, user.PasswordHash))
                    {
                        Console.WriteLine($"User authenticated: {user.Email}");
                        return user; 
                    }
                    else
                    {
                        Console.WriteLine($"Authentication failed: Incorrect password for {email}.");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine($"Authentication failed: User with email '{email}' not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user authentication: {ex.Message}");
                return null;
            }
        }

    
        public async Task<User?> CreatePasswordResetToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Reset token creation failed: Email cannot be empty.");
                return null;
            }

            
            var user = DatabaseHandler.Instance.SelectUserByEmail(email);
            if (user == null)
            {
                Console.WriteLine($"Reset token creation failed: User with email '{email}' not found.");
                return null;
            }

            try
            {
          
                string tokenValue = Guid.NewGuid().ToString("N");
                DateTime expiresAt = DateTime.UtcNow.AddHours(1); 

                
                int newTokenId = DatabaseHandler.Instance.InsertPasswordResetToken(user.UserID, tokenValue, expiresAt, false);
                Console.WriteLine($"Password reset token created for UserID {user.UserID}. Token: {tokenValue}");

               
                return new User(user.UserID, user.Email, user.PasswordHash, user.GoogleId)
                {
                    TokenID = newTokenId,
                    Token = tokenValue,
                    ExpiresAt = expiresAt,
                    Used = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password reset token creation: {ex.Message}");
                return null;
            }
        }

        
        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("Password reset failed: Token or new password cannot be empty.");
                return false;
            }

            
            using var conn = SQLiteConnector.GetConnection();
            using var transaction = conn.BeginTransaction();

            try
            {
              
                var userWithToken = DatabaseHandler.Instance.SelectPasswordResetToken(token);

                if (userWithToken == null || !userWithToken.IsValid())
                {
                    Console.WriteLine($"Password reset failed: Token '{token}' invalid or expired.");
                    transaction.Rollback();
                    return false;
                }

                
                string newHashedPassword = HashPassword(newPassword);
                
                DatabaseHandler.Instance.UpdateUserPassword(userWithToken.UserID, newHashedPassword);
                DatabaseHandler.Instance.UpdatePasswordResetTokenUsed(userWithToken.TokenID);

                transaction.Commit(); 
                Console.WriteLine($"Password successfully reset for UserID {userWithToken.UserID}.");
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback(); 
                Console.WriteLine($"Error during password reset: {ex.Message}");
                return false;
            }
        }
    }
}
