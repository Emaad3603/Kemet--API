using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
    using Kemet.Core.Entities.AI_Entites;
    using Kemet.Core.Entities.Identity;
    using Kemet.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;

    [Authorize]
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;
        private readonly UserManager<AppUser> _userManager;

        public AiController(IAiService aiService , UserManager<AppUser> userManager)
        {
            _aiService = aiService;
            _userManager = userManager;
        }

        [HttpPost("call")]
        public async Task<IActionResult> CallAiApi([FromBody] AiRequestDto requestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (string.IsNullOrEmpty(user.Id))
                return Unauthorized();

            var response = await _aiService.CallAiApiAsync(requestDto, user.Id);
            if (response == null)
                return BadRequest("AI API call failed.");

            return Ok(response);
        }
    }

}
