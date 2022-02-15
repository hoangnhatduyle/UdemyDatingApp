using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController] //this indicates this class is an API controller
    [Route("api/[controller]")] //this (controller) will be replaced by weatherforecast
    public class BaseApiController : ControllerBase
    {
        
    }
}