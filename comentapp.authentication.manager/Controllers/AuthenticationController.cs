using AutoMapper;
using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Provider;
using comentapp.authentication.businessLogic.Services;
using comentapp.persistence.Models;
using Comentapp.AuthenticationManager.Endpoint.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Comentapp.AuthenticationManager.Endpoint.Controllers
{
    /// <summary>
    /// Exposes the authentication and session endpoints: registration, email confirmation,
    /// local login, Google login, token refresh, logout, and current-user ("me") hydration.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(
        IMapper _mapper, 
        IAuthProviderFactory _authProviderFactory, 
        IUserService _userService, 
        ITokenService _tokenService,
        ICookieService _cookieService,
        IConfiguration _configuration) : ControllerBase
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

        /// <summary>
        /// Starts the Google OAuth challenge. The browser is redirected to Google's consent
        /// screen; on completion Google redirects back to <see cref="GoogleCallback"/>.
        /// </summary>
        /// <param name="returnUrl">Optional frontend-relative path to return to after login completes.</param>
        /// <returns>A challenge result that redirects the browser to Google.</returns>
        [HttpGet("google-login")]
        public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
        {
            var callbackUrl = Url.Action(nameof(GoogleCallback), values: new { returnUrl });

            var properties = new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the Google OAuth callback: reads the external principal from the
        /// <c>ExternalCookie</c> scheme, finds or creates the local user by verified email,
        /// issues the same app session cookies as local login, clears the external cookie,
        /// and redirects to an allowed frontend return URL.
        /// </summary>
        /// <param name="returnUrl">Optional frontend-relative path to return to after login completes.</param>
        /// <returns>A redirect to the frontend, on success or failure.</returns>
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl = null)
        {
            var externalResult = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!externalResult.Succeeded || externalResult.Principal is null)
            {
                await HttpContext.SignOutAsync("ExternalCookie");
                return Redirect(BuildFrontendRedirectUri(returnUrl, success: false));
            }

            var principal = externalResult.Principal;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                await HttpContext.SignOutAsync("ExternalCookie");
                return Redirect(BuildFrontendRedirectUri(returnUrl, success: false));
            }

            var name = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var surname = principal.FindFirst(ClaimTypes.Surname)?.Value;
            var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value;

            var googleLogin = new LoginDTO
            {
                User = new User
                {
                    Email = email,
                    Name = name ?? string.Empty,
                    Surname = surname ?? string.Empty,
                    UserName = givenName
                }
            };

            var provider = _authProviderFactory.GetProvider("google");
            var result = await provider.AuthenticateAsync(googleLogin);

            // La cookie externa ya cumplió su propósito: se descarta siempre,
            // tanto en éxito como en falla.
            await HttpContext.SignOutAsync("ExternalCookie");

            if (!result.IsSuccess)
                return Redirect(BuildFrontendRedirectUri(returnUrl, success: false));

            await _cookieService.SetAuthCookies(Response, result.Value);

            return Redirect(BuildFrontendRedirectUri(returnUrl, success: true));
        }

        /// <summary>
        /// Builds a safe redirect URI back to the frontend after the Google OAuth flow.
        /// Only relative paths are accepted for <paramref name="returnUrl"/> to prevent
        /// open-redirect attacks; anything else falls back to the frontend root.
        /// </summary>
        private string BuildFrontendRedirectUri(string? returnUrl, bool success)
        {
            var baseUrl = (_configuration["Frontend:BaseUrl"] ?? "http://localhost:5173").TrimEnd('/');

            var path = "/";
            if (!string.IsNullOrWhiteSpace(returnUrl)
                && returnUrl.StartsWith('/')
                && !returnUrl.StartsWith("//")
                && !returnUrl.Contains("://"))
            {
                path = returnUrl;
            }

            var separator = path.Contains('?') ? "&" : "?";
            var status = success ? "success" : "error";

            return $"{baseUrl}{path}{separator}googleAuth={status}";
        }
    }
}
