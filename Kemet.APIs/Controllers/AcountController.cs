﻿using AutoMapper;
using Kemet.APIs.DTOs.IdentityDTOs;
using Kemet.APIs.Errors;
using Kemet.APIs.Extensions;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.WishlistEntites;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Core.Services.InterFaces;
using Kemet.Repository.Data;
using Kemet.Repository.Data.DataSeed.DataSeedingDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Org.BouncyCastle.Bcpg;
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
        private readonly AppDbContext _context;
        private readonly IEmailSettings _emailSettings;
        private readonly OtpExtensions _otpExtensions;
       

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenServices tokenServices,
            AppDbContext context,
            IEmailSettings emailSettings,
            OtpExtensions otpExtensions
          
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _context = context;
            _emailSettings = emailSettings;
            _otpExtensions = otpExtensions;
            
        }

        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<UserDto>> RegisterCustomer(CustomerRegisterDTO model)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return BadRequest(new ApiResponse(400, "Email already exists"));
                }
                if (!await _userManager.CheckUserNameExistsAsync(model.UserName))
                {
                    return BadRequest(new ApiResponse(400, "Username already exists"));
                }

                var customer = new Customer
                {
                    UserName = model.UserName,
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterDTOs model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var admin = new Admin
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(admin, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                await _userManager.AddToRoleAsync(admin, "Admin");

                return Ok(new { Message = "Admin registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("RegisterTravelAgency")]
        public async Task<ActionResult<UserDto>> RegisterTravelAgency(TravelAgencyRegisterDTO model)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return BadRequest(new ApiResponse(400, "Email already exists"));
                }

                var travelAgency = new TravelAgency()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Description = model.Description,
                    TaxNumber = model.TaxNumber,
                    FacebookURL = model.FacebookURL,
                    InstagramURL = model.InstagramURL,
                    Bio = model.Bio
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(SignInDTO model)
        {
            try
            {
                var user = (Customer)await _userManager.FindByEmailAsync(model.Email);
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("AdminLogin")]
        public async Task<ActionResult<UserDto>> AdminLogin(SignInDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) return Unauthorized(new ApiResponse(401));

                // Ensure the user is an admin
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin) return Unauthorized(new ApiResponse(401, "User is not an admin"));

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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> GenerateAndSendOtp(ForgotPasswordRequestDTO request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound(new ApiResponse(404, "User not found"));
                }
                //otp created and saved to database
                var otp = await _otpExtensions.CreateOTP(user.Id);

                //create email 
                var email = new Email()
                {
                    Recipients = user.Email,
                    Subject = "Password Reset OTP",
                    Body = $"Your OTP code is: {otp.OTPValue}"
                };
                // Send OTP via email
                await _emailSettings.SendEmailAsync(email);

                var result = new ForgotPasswordResponseDTO()
                {
                    ApiResponse = new ApiResponse(200, "OTP generated and sent successfully"),
                    Data = user.Id
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        // Endpoint to verify OTP and generate reset token
        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDTO request)
        {
            try
            {
                var isValid = await _otpExtensions.VerifyOTP(request);

                if (!isValid)
                {
                    return Unauthorized(new ApiResponse(401, "Invalid or expired OTP"));
                }
                // Generate password reset token
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new ApiResponse(404, "User not found"));
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if (resetToken == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An error occurred while processing your request: "));
                }
                var result = new ForgotPasswordResponseDTO()
                {
                    ApiResponse = new ApiResponse(200, "OTP verified successfully"),
                    Data = resetToken 
                };

                return Ok(new { Message = "OTP verified successfully", ResetToken = resetToken });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        // Endpoint to reset password
        [HttpPost("Resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return NotFound(new ApiResponse(404, "User not found"));
                }

                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "Failed to change your password"));
                }

                return Ok(new { Message = "Password has been reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationDto locationDto)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
                if (user == null) return NotFound("User not found.");

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                user.Location = geometryFactory.CreatePoint(new Coordinate(locationDto.Longitude, locationDto.Latitude));

                await _userManager.UpdateAsync(user);
                return Ok(new { message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("TravelAgencyLogin")]
        public async Task<ActionResult<UserDto>> TravelAgencyLogin(SignInDTO model)
        {
            try
            {
                var user = (TravelAgency)await _userManager.FindByEmailAsync(model.Email);
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
    }

}
