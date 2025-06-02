namespace UniCodeProject.API.Contracts
{
    public interface IEmailService
    {
        Task SendRegistrationEmailAsync(string email, string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
