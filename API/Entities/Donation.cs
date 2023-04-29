using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public int NoOfMeals { get; set; }
        public string Status { get; set; } // "Availabe", "Collected", "Donated"
        public string FeedbackByDonor { get; set; }
        public string FeedbackByCollector { get; set; }
        
        [ForeignKey("Users")]
        public int DonorId { get; set; }

        [ForeignKey("Users")]
        public Nullable<int> CollectorId { get; set; }
    }
}