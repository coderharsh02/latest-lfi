namespace API.DTOs
{
    public class FullUserDetailsDto
    {
        public UserDetailDto User { get; set; }
        public List<DonationDto> Donations { get; set; }
        public List<DonationDto> Collections { get; set; }
    }
}