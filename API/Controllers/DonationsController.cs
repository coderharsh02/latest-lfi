using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DonationsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IRepository _repository;
        public DonationsController(DataContext context, IRepository repository)
        {
            _repository = repository;
            _context = context;
        }

        // GET api/donations
        [HttpGet]
        public async Task<ActionResult<List<DonationDto>>> GetDonations()
        {
            return Ok(await _repository.GetDonationsAsync());
        }

        [AllowAnonymous]
        [HttpGet("top")]
        public async Task<ActionResult<TopDonorCollectorDto>> GetTopDonorCollector()
        {
            return Ok(await _repository.GetTopDonorCollector());
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<DonationDto>> GetDonationById(int id)
        {
            var res = await _repository.GetDonationByIdAsync(id);
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpGet("userid/{userId}")]
        public async Task<ActionResult<List<DonationDto>>> GetDonationsByDonorId(int donorId)
        {
            return Ok(await _repository.GetDonationsByDonorIdAsync(donorId));
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<List<DonationDto>>> GetDonationsByUserName(string username)
        {
            int id = (await _repository.GetUserByUsernameAsync(username)).UserId;
            return await GetDonationsByDonorId(id);
        }

        [HttpPost]
        public async Task<ActionResult<Donation>> AddDonation(DonationFormDto dfd)
        {
            var donation = new Donation
            {
                NoOfMeals = dfd.NoOfMeals,
                Status = "Available",
                DonorId = dfd.DonorId,
            };

            // Add the user to the database.f
            _context.Donations.Add(donation);

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // return the UserTokenDto
            return donation;
        }

        [HttpPut("addcollector")]
        public async Task<ActionResult> AddCollector(Donation d)
        {
            var donation = await _repository.GetDonationEByIdAsync(d.Id);

            if (donation == null) return NotFound();

            if (donation.Status != "Available") return BadRequest("Donation is not available");

            donation.Status = "Booked";
            donation.CollectorId = d.CollectorId;
            donation.FeedbackByCollector = d.FeedbackByCollector;
            donation.FeedbackByDonor = d.FeedbackByDonor;

            _repository.UpdateDonation(donation);

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update donation");
        }

        [HttpPut("updateDonationStatusToCollected")]
        public async Task<ActionResult> updateDonationStatusToCollected(Donation donation1)
        {
            var donation = await _repository.GetDonationEByIdAsync(donation1.Id);

            if (donation == null) return NotFound();

            if (donation.Status == "Collected") return BadRequest("Donation is already Collected!");

            donation.Status = "Collected";
            donation.FeedbackByCollector = donation1.FeedbackByCollector;
            donation.FeedbackByDonor = donation1.FeedbackByDonor;
            _repository.UpdateDonation(donation);

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update donation");
        }

        [HttpPut("updateDonationStatusToDonated")]
        public async Task<ActionResult> updateDonationStatusToDonated(Donation donation1)
        {
            var donation = await _repository.GetDonationEByIdAsync(donation1.Id);

            if (donation == null) return NotFound();

            if (donation.Status == "Donated") return BadRequest("Donation is already donated!");

            donation.Status = "Donated";
            donation.FeedbackByCollector = donation1.FeedbackByCollector;
            donation.FeedbackByDonor = donation1.FeedbackByDonor;
            _repository.UpdateDonation(donation);

            if (await _repository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update donation");
        }
    }
}