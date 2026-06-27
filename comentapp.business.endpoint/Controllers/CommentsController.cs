using AutoMapper;
using comentapp.business.endpoint.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comentapp.business.endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController(
        IMapper _mapper
        ): ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Identidad funcionando correctamente");
        }

        [HttpPost]
        public IActionResult Post([FromBody] CommentRequest request)
        {
            // Aquí puedes agregar la lógica para manejar el comentario recibido

            return Ok(new
            {
                message = "Comentario recibido correctamente.",
                comment = request.Comment
            });
        }
    }
}
