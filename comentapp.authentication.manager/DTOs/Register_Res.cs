namespace Comentapp.AuthenticationManager.Endpoint.DTOs
{
    /// <summary>
    /// Response contract for <c>POST /Authentication/register</c>.
    /// Deliberately excludes sensitive fields (e.g. password hash) present on the EF entity.
    /// </summary>
    public class Register_Res
    {
        /// <summary>Unique identifier of the newly created user.</summary>
        public int Id { get; set; }

        /// <summary>First name of the user.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Last name of the user.</summary>
        public string Surname { get; set; } = string.Empty;

        /// <summary>Unique username of the user, if set.</summary>
        public string? UserName { get; set; }

        /// <summary>Email address of the user.</summary>
        public string Email { get; set; } = string.Empty;
    }
}
