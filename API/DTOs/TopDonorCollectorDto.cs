namespace API.DTOs
{
    public class TopDonorCollectorDto
    {
        public List<TopUserDto> TopDonors { get; set; } = new List<TopUserDto>();
        public List<TopUserDto> TopCollectors { get; set; } = new List<TopUserDto>();
    }
}