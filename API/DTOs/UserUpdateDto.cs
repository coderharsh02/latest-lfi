using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserUpdateDto
    {
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