namespace comentapp.infrastructure
{
    public class EmailOptions
    {
        public const string Section = "Email";

        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public bool UseSsl { get; init; } = false;
        public bool UseStartTls { get; init; } = true;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FromAddress { get; init; } = string.Empty;
        public string FromName { get; init; } = string.Empty;
    }
}
