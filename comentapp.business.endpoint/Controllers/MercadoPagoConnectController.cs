using System.Security.Claims;
using comentapp.business.endpoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace comentapp.business.endpoint.Controllers
{
    /// <summary>
    /// Flujo de conexión de la cuenta Mercado Pago del creador (paso 3).
    /// Ruta base <c>/MercadoPago</c> alineada con el spec (comparte prefijo con el webhook).
    /// </summary>
    [Route("MercadoPago")]
    [ApiController]
    public class MercadoPagoConnectController(
        IMercadoPagoConnectService connectService,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Inicia la conexión OAuth del creador autenticado. Devuelve la URL de autorización de MP.
        /// Same-site: la cookie de sesión (Strict) viaja normalmente.
        /// </summary>
        [Authorize]
        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var result = await connectService.StartConnectAsync(userId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new { authorizationUrl = result.AuthorizationUrl });
        }

        /// <summary>
        /// Callback de Mercado Pago. Cross-site (sin cookie de sesión): la identidad viaja en el state.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state)
        {
            var result = await connectService.HandleCallbackAsync(code ?? string.Empty, state ?? string.Empty);

            var frontendBase = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var status = result.Success ? "success" : "error";
            return Redirect($"{frontendBase}/creator/mercadopago?connect={status}");
        }

        /// <summary>Estado de conexión MP del creador autenticado.</summary>
        [Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var status = await connectService.GetStatusAsync(userId);
            return Ok(new
            {
                isCreator = status.IsCreator,
                connected = status.Connected,
                accountId = status.AccountId,
            });
        }

        /// <summary>Desconecta la cuenta MP del creador autenticado.</summary>
        [Authorize]
        [HttpDelete("connection")]
        public async Task<IActionResult> Disconnect()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var removed = await connectService.DisconnectAsync(userId);
            return removed ? NoContent() : NotFound();
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
        }
    }
}
