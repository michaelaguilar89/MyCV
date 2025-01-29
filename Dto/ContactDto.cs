using System.ComponentModel.DataAnnotations;

namespace MICV.Dto
{
    public class ContactDto
    {
        // Información del Cliente
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

   
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }



    }
}
