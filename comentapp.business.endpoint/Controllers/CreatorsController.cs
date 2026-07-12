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
    public class CreatorsController(ICreatorService creatorService) : ControllerBase
    {
        /// <summary>Paso 2: registra al usuario autenticado como creador.</summary>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateCreatorRequest request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var result = await creatorService.RegisterAsync(userId, request.CreatorName.Trim());
            if (!result.Success)
            {
                return result.Error switch
                {
                    CreatorErrorCode.AlreadyCreator => Conflict(new { error = "El usuario ya es creador." }),
                    CreatorErrorCode.NameTaken => Conflict(new { error = "El nombre de creador ya está en uso." }),
                    _ => BadRequest(),
                };
            }

            return CreatedAtAction(nameof(GetMe), null, ToResponse(result));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var result = await creatorService.GetByUserIdAsync(userId);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(ToResponse(result));
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId);
        }

        private static CreatorResponse ToResponse(CreatorResult result)
        {
            var creator = result.Creator!;
            return new CreatorResponse
            {
                Id = creator.Id,
                CreatorName = creator.CreatorName,
                UserId = creator.UserId,
                Description = creator.Description,
                MercadoPagoConnected = result.IsConnected,
            };
        }
    }
}
