using API.DTOs;
using API.Entities;
using API.Errors;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;
        public Repository(DataContext context)
        {
            _context = context;
        }

        ////////////////////////////////////////////////// Users /////////////////////////////////////////////////////////////////////////

        // Return List of Users (Donations and Collections is not included) 
        public async Task<List<UserDetailDto>> GetUsersAsync()
        {
            return UserDetailDtoFromAppUser(await _context.Users.ToListAsync());
        }

        // Takes userId and Return User if Exist else throw 404 Not Found (Donations and Collections is not included) 
        public async Task<UserDetailDto> GetUserByIdAsync(int? userId)
        {
            if (userId == null) throw new ApiException(404, "UserId Cannot be null");

            AppUser user = await _context.Users.FindAsync(userId);

            if (user == null) throw new ApiException(404, "User Not Found");

            return UserDetailDtoFromAppUser(user);
        }

        public async Task<AppUser> GetAppUserByUsernameAsync(string username)
        {
            var user = await _context.Users.Where(p => p.UserName == username).SingleOrDefaultAsync();

            if (user == null) throw new ApiException(404, "User Not Found");

            return user;
        }

        // Takes UserName and Return User if Exist else throw 404 Not Found (Donations and Collections is not included) 
        public async Task<UserDetailDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users.Where(p => p.UserName == username).SingleOrDefaultAsync();

            if (user == null) throw new ApiException(404, "User Not Found");

            return UserDetailDtoFromAppUser(user);
        }

        public UserDetailDto UserDetailDtoFromAppUser(AppUser user)
        {
            return new UserDetailDto()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                DonorType = user.DonorType,
                VolunteerType = user.VolunteerType,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                City = user.City,
                Pincode = user.Pincode,
                PhoneNumber = user.PhoneNumber,
                PhotoUrl = user.PhotoUrl
            };
        }
        public List<UserDetailDto> UserDetailDtoFromAppUser(List<AppUser> appUsersList)
        {
            List<UserDetailDto> userDetailDtoList = new List<UserDetailDto>();
            foreach (AppUser user in appUsersList)
            {
                userDetailDtoList.Add(UserDetailDtoFromAppUser(user));
            }
            return userDetailDtoList;
        }

        public async Task<List<FullUserDetailsDto>> GetFullUsersAsync()
        {
            List<FullUserDetailsDto> FullUserDetailsDtoList = new List<FullUserDetailsDto>();
            foreach (AppUser user in _context.Users.ToList())
            {
                FullUserDetailsDtoList.Add(await GetFullUserByIdAsync(user.Id));
            }
            return FullUserDetailsDtoList;
        }
        public async Task<FullUserDetailsDto> GetFullUserByIdAsync(int userId)
        {
            FullUserDetailsDto fudd = new FullUserDetailsDto();
            fudd.User = await GetUserByIdAsync(userId);
            fudd.Donations = await GetDonationsByDonorIdAsync(userId);
            foreach (var donation in fudd.Donations)
            {
                donation.DonatedBy = null;
            }
            fudd.Collections = await GetDonationsByCollectorIdAsync(userId);
            foreach (var collection in fudd.Collections)
            {
                collection.CollectedBy = null;
            }
            return fudd;
        }

        public async Task<FullUserDetailsDto> GetFullUserByUsernameAsync(string username)
        {
            var user = await _context.Users.Where(p => p.UserName == username).SingleOrDefaultAsync();

            if (user == null) throw new ApiException(404, "User Not Found");

            return await GetFullUserByIdAsync(user.Id);
        }

        ////////////////////////////////////////////////// Donations /////////////////////////////////////////////////////////////////////////

        public async Task<TopDonorCollectorDto> GetTopDonorCollector()
        {
            var topUsers = new TopDonorCollectorDto();

            var topDonors = await _context.Donations.GroupBy(l => l.DonorId)
            .Select(cl => new
            {
                DonorId = cl.First().DonorId,
                NoOfMeals = cl.Sum(c => c.NoOfMeals),
            }).ToListAsync();
            topDonors = topDonors.OrderByDescending(x => x.NoOfMeals).Take(3).ToList();

            if(topDonors.Count > 0)
            {
                foreach (var donor in topDonors)
                {
                    TopUserDto temp = new TopUserDto();
                    temp.User = await GetUserByIdAsync(donor.DonorId);
                    temp.NoOfMeals = donor.NoOfMeals;
                    topUsers.TopDonors.Add(temp);
                }
            }

            var topCollectors = await _context.Donations.GroupBy(l => l.CollectorId)
            .Select(cl => new
            {
                CollectorId = cl.First().CollectorId,
                NoOfMeals = cl.Sum(c => c.NoOfMeals),
            }).Where(x => x.CollectorId != null).ToListAsync();
            topCollectors = topCollectors.OrderByDescending(x => x.NoOfMeals).Take(3).ToList();
            
            if(topCollectors.Count > 0)
            {
                foreach (var collection in topCollectors)
                {
                    TopUserDto temp = new TopUserDto();
                    temp.User = await GetUserByIdAsync(collection.CollectorId);
                    temp.NoOfMeals = collection.NoOfMeals;
                    topUsers.TopCollectors.Add(temp);
                }
            }

            return topUsers;
        }

        // Return List of DonationDto (DonatedBy and CollectedBy is included of Type UserDto)
        public async Task<List<DonationDto>> GetDonationsAsync()
        {
            return await DonationDtoFromDonation(await _context.Donations.ToListAsync());
        }

        // Takes donationId and Return DonationDto if Exist else return null (DonatedBy and CollectedBy is included of Type UserDto)
        public async Task<DonationDto> GetDonationByIdAsync(int donationId)
        {
            var donation = await _context.Donations.FindAsync(donationId);
            if (donation == null) return null;
            return await DonationDtoFromDonation(donation);
        }

        public async Task<Donation> GetDonationEByIdAsync(int donationId)
        {
            var donation = await _context.Donations.FindAsync(donationId);
            if (donation == null) return null;
            return donation;
        }

        // Takes donorId and Return List of DonationDto that have same donorId (DonatedBy and CollectedBy is included of Type UserDto)
        public async Task<List<DonationDto>> GetDonationsByDonorIdAsync(int donorId)
        {
            // var donation = await _context.Donations.Where(p => p.DonorId == donorId);

            var donations = from donation in _context.Donations
                            where donation.DonorId == donorId
                            select donation;

            if (donations == null) return null;

            return await DonationDtoFromDonation(await _context.Donations.Where(p => p.DonorId == donorId).ToListAsync());
        }

        public async Task<List<DonationDto>> GetDonationsByCollectorIdAsync(int collectorId)
        {
            return await DonationDtoFromDonation(await _context.Donations.Where(p => p.CollectorId == collectorId).ToListAsync());
        }

        public async Task<DonationDto> DonationDtoFromDonation(Donation donation)
        {
            return new DonationDto()
            {
                DonationId = donation.Id,
                NoOfMeals = donation.NoOfMeals,
                Status = donation.Status,
                DonatedBy = await GetUserByIdAsync(donation.DonorId),
                CollectedBy = (donation.CollectorId != null) ? await GetUserByIdAsync((donation.CollectorId)) : null,
                FeedbackByCollector = donation.FeedbackByCollector,
                FeedbackByDonor = donation.FeedbackByDonor,
            };
        }
        public async Task<List<DonationDto>> DonationDtoFromDonation(List<Donation> donationList)
        {
            List<DonationDto> DonationDtoList = new List<DonationDto>();
            foreach (Donation donation in donationList)
            {
                DonationDtoList.Add(await DonationDtoFromDonation(donation));
            }
            return DonationDtoList;
        }

        public void UpdateUser(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public void UpdateDonation(Donation donation)
        {
            _context.Entry(donation).State = EntityState.Modified;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}