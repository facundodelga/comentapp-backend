using AutoMapper;
using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Provider;
using comentapp.authentication.businessLogic.Services;
using comentapp.authentication.businessLogic.Services.Implementation;
using Comentapp.AuthenticationManager.Endpoint.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Comentapp.AuthenticationManager.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(
        IMapper _mapper, 
        IAuthProviderFactory _authProviderFactory, 
        IUserService _userService, 
        ITokenService _tokenService,
        ICookieService _cookieService) : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Identidad funcionando correctamente");
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] Login_Req request)
        {
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

            _cookieService.SetAuthCookies(Response, result.Value);

            return Ok(result);
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register_Req request)
        {
            var requestDto = _mapper.Map<RegisterDTO>(request);
            var result = await _userService.RegisterUser(requestDto);

            if(result.IsSuccess)
                return Ok(result.Value);

            if (result.ErrorCode == (int)UserServiceErrorCodes.CU_EmailAlreadyExists 
                || result.ErrorCode == (int)UserServiceErrorCodes.CU_UsernameAlreadyExists)
                return Conflict(new { Message = result.ErrorMessage });

            return StatusCode(500, result.ErrorMessage);
        }


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
                _cookieService.ClearAuthCookies(Response);
                return Unauthorized(result.ErrorMessage);
            }

            _cookieService.SetAuthCookies(Response, result.Value);

            return Ok(new { message = "Token renovado." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = _cookieService.GetRefreshToken(Request);

            if (!string.IsNullOrEmpty(refreshToken))
                await _tokenService.RevokeAsync(refreshToken);

            _cookieService.ClearAuthCookies(Response);

            return NoContent();
        }
    }
}
