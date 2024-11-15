using AutoMapper;
using Kemet.APIs.DTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.InterFaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kemet.APIs.Controllers
{
   
    // AccountsController Class
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenServices tokenServices,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _mapper = mapper;
        }

        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<UserDto>> RegisterCustomer(CustomerRegisterDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return BadRequest(new ApiResponse(400, "Email already exists"));
            }

            var customer = new Customer
            {
                UserName = string.Concat(model.FirstName, model.LastName),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                SSN = model.SSN,
                Nationality = model.Nationality,
                Gender = model.Gender 
            };

            var result = await _userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            await _userManager.AddToRoleAsync(customer, "Customer");

            var userDto = new UserDto
            {
                UserName = customer.UserName,
                Email = customer.Email,
                Token = await _tokenServices.CreateTokenAsync(customer, _userManager)
            };
            return Ok(userDto);
        }

        [HttpPost("RegisterTravelAgency")]
        public async Task<ActionResult<UserDto>> RegisterTravelAgency(TravelAgencyRegisterDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return BadRequest(new ApiResponse(400, "Email already exists"));
            }

            var travelAgency = new TravelAgency
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Description = model.Description,
                TaxNumber = model.TaxNumber,
                FacebookURL = model.FacebookURL,
                InstagramURL = model.InstagramURL
            };

            var result = await _userManager.CreateAsync(travelAgency, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            await _userManager.AddToRoleAsync(travelAgency, "TravelAgency");

            var userDto = new UserDto
            {
                UserName = travelAgency.UserName,
                Email = travelAgency.Email,
                Token = await _tokenServices.CreateTokenAsync(travelAgency, _userManager)
            };
            return Ok(userDto);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(SignInDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = await _tokenServices.CreateTokenAsync(user, _userManager);

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            });
        }
    }

}
