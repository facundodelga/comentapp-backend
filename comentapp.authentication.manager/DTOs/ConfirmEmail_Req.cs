namespace Comentapp.AuthenticationManager.Endpoint.DTOs
{
    public class ConfirmEmail_Req
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

    }
}
