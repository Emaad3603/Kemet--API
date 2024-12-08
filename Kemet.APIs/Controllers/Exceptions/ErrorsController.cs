using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kemet.APIs.Errors;

namespace Kemet.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {

        public IActionResult Error (int code)
        {
            return NotFound(new ApiResponse(code));
        }

    }
} 
