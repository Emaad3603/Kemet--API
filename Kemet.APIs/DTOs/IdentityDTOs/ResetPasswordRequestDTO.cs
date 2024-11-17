namespace Kemet.APIs.DTOs.IdentityDTOs
{
    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; }
        public string Token { get; set; } // Token received from OTP verification
        public string NewPassword { get; set; }
    }
}
