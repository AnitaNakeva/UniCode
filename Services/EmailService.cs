using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly UniCodeProject.API.Contracts.IEmailSender _emailSender;

        public EmailService(ApplicationDbContext dbContext, IConfiguration configuration, UniCodeProject.API.Contracts.IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task SendRegistrationEmailAsync(string email, string userId)
        {
            Console.WriteLine($"SendRegistrationEmailAsync called with Email: {email}, UserId: {userId}");

            // Generate a secure token
            var token = Guid.NewGuid().ToString();
            var expiryDate = DateTime.UtcNow.AddHours(24);

            // Save the token to the database
            var emailConfirmationToken = new EmailConfirmationToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiryDate = expiryDate
            };
            _dbContext.EmailConfirmationTokens.Add(emailConfirmationToken);
            await _dbContext.SaveChangesAsync();

            // Generate the confirmation link
            var confirmationLink = $"{_configuration["FrontendUrl"]}/api/email/confirm-email?userId={userId}&token={token}";

            // Construct the email content
            var subject = "Confirm Your Registration";
            var message = $@"
            <h1>Welcome to UniCode!</h1>
            <p>Click the link below to confirm your email address:</p>
            <a href='{confirmationLink}'>Confirm Email</a>
            <p>If you did not sign up for this account, please ignore this email.</p>";

            // Send the email
            await _emailSender.SendEmailAsync(email, subject, message);

            Console.WriteLine($"Email sent successfully.");

        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            // Validate the token from the database
            var emailConfirmationToken = await _dbContext.EmailConfirmationTokens
                .FirstOrDefaultAsync(e => e.UserId == userId && e.Token == token);

            if (emailConfirmationToken == null || emailConfirmationToken.ExpiryDate < DateTime.UtcNow)
            {
                return false;
            }

            // Retrieve the user
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Mark the email as confirmed
            user.EmailConfirmed = true;
            _dbContext.EmailConfirmationTokens.Remove(emailConfirmationToken);
            await _dbContext.SaveChangesAsync();

            return true;
        }

    }

}
