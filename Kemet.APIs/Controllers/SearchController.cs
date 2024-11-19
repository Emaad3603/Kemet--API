using Kemet.APIs.Errors;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
  
    public class SearchController : BaseApiController
    {
        private readonly ISearchInterface _searchMethod;

        public SearchController(ISearchInterface searchMethod)
        {
            _searchMethod = searchMethod;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> RagionalSearch(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var result = await _searchMethod.SearchAll(text);
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
