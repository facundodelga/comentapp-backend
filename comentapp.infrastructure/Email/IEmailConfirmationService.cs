namespace comentapp.infrastructure.Email
{
    public interface IEmailConfirmationService
    {
        string GenerateToken(int userId, string email);
        int? ValidateToken(string token, string email);
    }
}
