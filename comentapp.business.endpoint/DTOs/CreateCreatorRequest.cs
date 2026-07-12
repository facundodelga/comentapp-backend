using System.ComponentModel.DataAnnotations;

namespace comentapp.business.endpoint.DTOs
{
    /// <summary>
    /// Paso 2: registrarse como creador. Solo pide el nombre público (único).
    /// description/links/página son del paso 3 (feature futura).
    /// </summary>
    public class CreateCreatorRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string CreatorName { get; set; } = string.Empty;
    }
}
