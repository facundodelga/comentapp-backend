using AutoMapper;
using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Provider;
using comentapp.authentication.businessLogic.Services;
using Comentapp.AuthenticationManager.Endpoint.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Comentapp.AuthenticationManager.Endpoint.Controllers
{
    /// <summary>
    /// Exposes the authentication and session endpoints: registration, email confirmation,
    /// local login, token refresh, logout, and current-user ("me") hydration.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(
        IMapper _mapper, 
        IAuthProviderFactory _authProviderFactory, 
        IUserService _userService, 
        ITokenService _tokenService,
        ICookieService _cookieService) : ControllerBase
    {
        /// <summary>
        /// Lightweight health-check for the authenticated identity pipeline.
        /// </summary>
        /// <returns>200 OK when the caller has a valid session.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("Identidad funcionando correctamente");
        }

        /// <summary>
        /// Confirms a user's email address using the token issued at registration.
        /// </summary>
        /// <param name="request">The email and confirmation token.</param>
        /// <returns>200 OK on success; 400 Bad Request if the token is invalid/expired or already confirmed.</returns>
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmail_Req request)
        {
            var requestDto = _mapper.Map<ConfirmMailDTO>(request);
            var result = await _userService.ConfirmEmailAsync(requestDto);

            if(result.IsSuccess)
                return Ok(new
                {
                    message = "Email confirmado correctamente."
                });

            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Authenticates a user with email/password and starts an HTTP-only cookie session.
        /// </summary>
        /// <param name="request">The login credentials.</param>
        /// <returns>200 OK with session tokens on success; 400 Bad Request on invalid input; 401 Unauthorized on invalid credentials or unconfirmed email.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Login_Req request)
        {
            var validation = new Login_Req_Validation().Validate(request);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var requestDto = _mapper.Map<LoginDTO>(request);
            
            var provider = _authProviderFactory.GetProvider("local");
            var result = await provider.AuthenticateAsync(requestDto);

            if (!result.IsSuccess)
            {
                return Unauthorized(new
                {
                    message = result.ErrorMessage
                });
            }

            await _cookieService.SetAuthCookies(Response, result.Value);

            return Ok(result);
        }

        /// <summary>
        /// Registers a new user account and sends an email-confirmation link.
        /// </summary>
        /// <param name="request">The registration data.</param>
        /// <returns>200 OK with the created user's public profile; 400 Bad Request on invalid input; 409 Conflict if the email/username is already taken.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register_Req request)
        {
            var validation = new Register_Req_Validation().Validate(request);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var requestDto = _mapper.Map<RegisterDTO>(request);
            var result = await _userService.RegisterUser(requestDto);

            if (result.IsSuccess)
                return Ok(_mapper.Map<Register_Res>(result.Value));

            if (result.ErrorCode == (int)UserServiceErrorCodes.CU_EmailAlreadyExists 
                || result.ErrorCode == (int)UserServiceErrorCodes.CU_UsernameAlreadyExists)
                return Conflict(new { Message = result.ErrorMessage });

            return StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Rotates the current session's refresh token (read from the auth cookie) and
        /// issues a new access/refresh token pair.
        /// </summary>
        /// <returns>200 OK on success; 401 Unauthorized when no valid session/refresh token exists.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // El refresh token viene de la cookie — no del body
            var refreshToken = _cookieService.GetRefreshToken(Request);

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("No hay sesión activa.");

            var result = await _tokenService.RefreshAsync(refreshToken);

            if (!result.IsSuccess)
            {
                await _cookieService.ClearAuthCookies(Response);
                return Unauthorized(result.ErrorMessage);
            }

            await _cookieService.SetAuthCookies(Response, result.Value);

            return Ok(new { message = "Token renovado." });
        }

        /// <summary>
        /// Revokes the current refresh token (if any) and clears the session cookies.
        /// </summary>
        /// <returns>204 No Content.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = _cookieService.GetRefreshToken(Request);

            if (!string.IsNullOrEmpty(refreshToken))
                await _tokenService.RevokeAsync(refreshToken);

            await _cookieService.ClearAuthCookies(Response);

            return NoContent();
        }

        /// <summary>
        /// Returns the full profile of the currently authenticated user, used by the frontend
        /// to hydrate session state and route guards.
        /// </summary>
        /// <returns>200 OK with the user's <see cref="Me_Res"/> profile; 401 Unauthorized if not authenticated; 404 Not Found if the user no longer exists.</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Usuario no autenticado");

            var result = await _userService.GetCurrentUserAsync(userId);

            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return Ok(_mapper.Map<Me_Res>(result.Value));
        }
    }
}
