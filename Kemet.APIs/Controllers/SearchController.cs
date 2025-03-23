using Kemet.APIs.Errors;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
  
    public class SearchController : BaseApiController
    {
        private readonly ISearchInterface _searchMethod;
        private readonly UserManager<AppUser> _userManager;

        public SearchController(
            ISearchInterface searchMethod ,
            UserManager<AppUser> userManager)
        {
            _searchMethod = searchMethod;
            _userManager = userManager;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> RagionalSearch(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var result = await _searchMethod.SearchAll(text , _userManager);
                    return Ok(result);
                }
                return BadRequest(new ApiResponse(404, "search text can be empty"));
            }catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
         
        }
    }
}
