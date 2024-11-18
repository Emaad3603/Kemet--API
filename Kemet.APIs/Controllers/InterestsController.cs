using Kemet.APIs.DTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.Intersts;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Repository.Repositories;
using Kemet.Repository.Specification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kemet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerInterestsController : ControllerBase
    {
        private readonly IinterestsRepository _repository;
        private readonly UserManager<AppUser> _userManager;

        public CustomerInterestsController(
            IinterestsRepository repository,
            UserManager<AppUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet("GetInterests")]
        public async Task<IActionResult> GetInterestsByCustomerId()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userEmail);
                var spec = new CustomerInterestsSpecification(user.Id);
                var interests = await _repository.GetAllWithSpecAsync(spec);
                
                if (interests == null || interests.Count == 0)
                {
                    return NotFound(new ApiResponse(404, "No interests found for this customer."));
                }  
                var userInterests = await _repository.GetUserInterestsAsync(user.Id);
                return Ok(userInterests);

            }catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("AddInterests")]
        public async Task<IActionResult> AddInterest( CustomerInterestsDto interests)
        {
            if (interests == null)
            {
                return BadRequest(new ApiResponse(400, "Interest cannot be null."));
            }

            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userEmail);

                var categoriesIDs = interests.CategoryIds;
                await _repository.AddInterestsAsync(user.Id, categoriesIDs);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("UpdateInterests")]
        public async Task<IActionResult> UpdateInterest(CustomerInterestsDto interests)
        {
            var categoryIds = interests.CategoryIds;
            if (categoryIds == null || !categoryIds.Any())
            {
                return BadRequest(new ApiResponse(400, "Category IDs cannot be null or empty."));
            }

            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(userEmail);
                var interest = await _repository.GetUserInterestsAsync(user.Id);
                if (interest == null)
                {
                    return NotFound(new ApiResponse(404, "Interest not found."));
                }
                await  _repository.UpdateInterestsAsync(user.Id, categoryIds);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

     
    }
    
}