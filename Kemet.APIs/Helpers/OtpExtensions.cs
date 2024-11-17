using Kemet.APIs.DTOs.IdentityDTOs;
using Kemet.Core.Entities.Identity;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace Kemet.APIs.Helpers
{
    public class OtpExtensions
    {
        private readonly AppDbContext _context;

        public OtpExtensions(AppDbContext context)
        {
            _context = context;
        }
        // Method to Save OTP in DataBase
        public async Task<OTP> CreateOTP (string userID)
        {
            var otp = new OTP
            {
                OTPValue = GenerateRandomOtp(),
                CreatedAt = DateTime.UtcNow,
                IsUsed = false,
                UserId = userID
            };
            // Save OTP to database
            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }

        // Method to generate a random 6-digit OTP
        private string GenerateRandomOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<bool> VerifyOTP(VerifyOtpRequestDTO request)
        {
            var otp = await _context.OTPs.FirstOrDefaultAsync(o => o.UserId == request.UserId && o.OTPValue == request.OTP);

            if (otp == null || otp.IsUsed || otp.CreatedAt.AddMinutes(30) <= DateTime.UtcNow)
            {
                
                return false;

            }
            else
            {
                // Mark OTP as used
                otp.IsUsed = true;
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
