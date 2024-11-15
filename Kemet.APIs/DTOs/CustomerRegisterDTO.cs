using System.ComponentModel.DataAnnotations;

namespace Kemet.APIs.DTOs
{
    public class CustomerRegisterDTO
    {
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
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]       
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string SSN { get; set; }

        public string Gender { get; set; }
    }
}
