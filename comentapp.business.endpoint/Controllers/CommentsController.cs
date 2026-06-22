using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace comentapp.business.endpoint.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentsController(
        IMapper _mapper,
        IUserService _userService
        ): ControllerBase
    {

    }
}
