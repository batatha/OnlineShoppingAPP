namespace OnlineShoppingAPI.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmail(string toEmail, string resetToken);
    }

    public class EmailService : IEmailService
    {
        public async Task SendPasswordResetEmail(string toEmail, string resetToken)
        {

            // For Demo, printing the token
            Console.WriteLine($"Sending reset token {resetToken} to {toEmail}. Please check your email.");
        }
    }
}
