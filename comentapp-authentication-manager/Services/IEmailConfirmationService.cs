namespace comentapp_authentication_manager.Services
{
    public interface IEmailConfirmationService
    {
        string GenerateToken(int userId, string email);
        int? ValidateToken(string token, string email);
    }
}
