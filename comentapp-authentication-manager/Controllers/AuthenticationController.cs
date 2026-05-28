using AutoMapper;
using comentapp_authentication_manager.DTOs;
using comentapp_authentication_manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comentapp_authentication_manager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(IMapper _mapper) : ControllerBase
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
        public IActionResult Register([FromBody] Register_Req request)
        {
            var newUser = _mapper.Map<User>(request);

            return Ok(newUser);
        }
    }
}
