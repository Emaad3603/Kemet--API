using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kemet.APIs.DTOs
{
    public class TravelAgencyRegisterDTO
    {
          [Required]
          public string UserName { get; set; }
          [Required]
          public string Email { get; set; }
          public string Password { get; set; }
          [Required]
          [Phone]
          public string PhoneNumber { get; set; }
          [Required]
          public string Address { get; set; }
          public string? Description { get; set; }
          [Description]
          public string TaxNumber { get; set; }
          public string? FacebookURL { get; set; }
          public string? InstagramURL { get; set; }
    }
}
