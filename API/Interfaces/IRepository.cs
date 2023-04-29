using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IRepository
    {
        Task<List<UserDetailDto>> GetUsersAsync();
        public Task<UserDetailDto> GetUserByIdAsync(Nullable<int> userId);

        public Task<AppUser> GetAppUserByUsernameAsync(string username);
        public Task<UserDetailDto> GetUserByUsernameAsync(string username);

        public Task<List<FullUserDetailsDto>> GetFullUsersAsync();
        public Task<FullUserDetailsDto> GetFullUserByIdAsync(int userId);
        public Task<FullUserDetailsDto> GetFullUserByUsernameAsync(string username);

        // public Task<UserDetailDto> AddUser(UserDetailDto userDetailDto);
        // public Task<UserDetailDto> UpdateUser(UserDetailDto userDetailDto);
        // public Task<UserDetailDto> DeleteUser(Nullable<int> userId);

        public Task<TopDonorCollectorDto> GetTopDonorCollector();
        public Task<List<DonationDto>> GetDonationsAsync();
        public Task<DonationDto> GetDonationByIdAsync(int donationId);
        public Task<Donation> GetDonationEByIdAsync(int donationId);
        public Task<List<DonationDto>> GetDonationsByDonorIdAsync(int donorId);
        public Task<List<DonationDto>> GetDonationsByCollectorIdAsync(int collectorId);

        // public Task<IEnumerable<DonationDto>> GetDonations();
        // public Task<DonationDto> GetDonationById(Nullable<int> donationId);
        // public Task<DonationDto> AddDonation(DonationDto donationDto);
        // public Task<DonationDto> UpdateDonation(DonationDto donationDto);
        // public Task<DonationDto> DeleteDonation(Nullable<int> donationId);

        // public Task<IEnumerable<DonationDto>> GetDonationsByDonorId(Nullable<int> donorId);
        // public Task<IEnumerable<DonationDto>> GetDonationsByCollectorId(Nullable<int> collectorId);
        // public Task<IEnumerable<DonationDto>> GetDonationsByStatus(string status);

        public void UpdateUser(AppUser user);
        public void UpdateDonation(Donation donation);
        public Task<bool> SaveAllAsync();

    }
}