using System.Security.Claims;
using comentapp.business.endpoint.DTOs;
using comentapp.business.endpoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace comentapp.business.endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DonationCommentsController(IDonationCheckoutService checkoutService) : ControllerBase
    {
        /// <summary>
        /// Crea una intención de donación con comentario y devuelve la URL de checkout de Mercado Pago.
        /// El comentario queda sin confirmar hasta la verificación server-side del pago (webhook).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DonationCommentRequest request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return Unauthorized();
            }

            var result = await checkoutService.CreateAsync(userId, request);
            if (!result.Success)
            {
                return result.Error switch
                {
                    DonationErrorCode.CreatorNotFound => NotFound(new { error = "El creador no existe." }),
                    DonationErrorCode.CreatorNotConnected => UnprocessableEntity(new { error = "El creador no puede recibir donaciones todavía." }),
                    _ => StatusCode(502, new { error = "No se pudo iniciar el pago con Mercado Pago." }),
                };
            }

            return Ok(result.Response);
        }
    }
}
