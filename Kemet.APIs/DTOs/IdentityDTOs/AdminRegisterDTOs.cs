using System.ComponentModel.DataAnnotations;

namespace Kemet.APIs.DTOs.IdentityDTOs
{
    public class AdminRegisterDTOs
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string SSN { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
