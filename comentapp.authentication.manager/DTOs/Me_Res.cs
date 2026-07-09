namespace Comentapp.AuthenticationManager.Endpoint.DTOs
{
    /// <summary>
    /// Response contract for <c>GET /Authentication/me</c>, used by the frontend
    /// to hydrate the current session and route guards.
    /// </summary>
    public class Me_Res
    {
        /// <summary>Unique identifier of the authenticated user.</summary>
        public int Id { get; set; }

        /// <summary>First name of the user.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Last name of the user.</summary>
        public string Surname { get; set; } = string.Empty;

        /// <summary>Unique username of the user, if set.</summary>
        public string? UserName { get; set; }

        /// <summary>Email address of the user.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Whether the user owns an active creator profile.
        /// </summary>
        public bool IsCreator { get; set; }
    }
}
