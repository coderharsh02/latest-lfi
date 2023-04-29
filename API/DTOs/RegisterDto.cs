using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {

        // [Phone] attribute is used to validate the phone number.
        // [MaxLength(10)] attribute is used to validate the maximum length of the phone number.
        // [RegularExpression(@"^[0-9]*$")] attribute is used to validate the phone number.
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string DonorType { get; set; }
        public string VolunteerType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int Pincode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }

    }
}