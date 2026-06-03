using AutoMapper;
using comentapp_authentication_manager.Core;
using comentapp_authentication_manager.DTOs;
using comentapp_authentication_manager.Models;
using comentapp_authentication_manager.Repository;
using comentapp_authentication_manager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace comentapp_authentication_manager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(IUserService _userService) : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Identidad funcionando correctamente");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login_Req request)
        {
            // TODO: Implementar lógica de validación con base de datos y generación de JWT
            if (request.Email == "admin" && request.Password == "123456")
            {
                return Ok(new { Token = "ejemplo-de-token-jwt" });
            }

            return Unauthorized(new { Mensaje = "Credenciales incorrectas" });
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] Register_Req request)
        {
            var result = await _userService.RegisterUser(request);
            if(result.IsSuccess)
                return Ok(result.Value);

            if (result.ErrorCode == (int)UserServiceErrorCodes.CU_EmailAlreadyExists 
                || result.ErrorCode == (int)UserServiceErrorCodes.CU_UsernameAlreadyExists)
                return Conflict(new { Message = result.ErrorMessage });

            return StatusCode(500, result.ErrorMessage);
        }
    }
}
