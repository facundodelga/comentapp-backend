using AutoMapper;
using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services;
using Comentapp.AuthenticationManager.Endpoint.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Comentapp.AuthenticationManager.Endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(IMapper _mapper, IUserService _userService) : ControllerBase
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
            
            var result = await _userService.LoginUser(requestDto);

            if (!result.IsSuccess)
            {
                return Unauthorized(new
                {
                    message = "Credenciales inválidas"
                });
            }

            if (!result.Value.IsEmailConfirmed)
            {
                return Unauthorized(new
                {
                    message = "Correo electrónico no confirmado"
                });
            }

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
    }
}
