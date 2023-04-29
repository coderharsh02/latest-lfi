using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }
            await context.SaveChangesAsync();
        }

        public static async Task SeedDonations(DataContext context)
        {
            if (await context.Donations.AnyAsync()) return;
            var donationData = await File.ReadAllTextAsync("Data/DonationSeedData.json");
            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
            var donations = JsonSerializer.Deserialize<List<Donation>>(donationData);
            foreach (var donation in donations)
            {
                context.Donations.Add(donation);
            }
            await context.SaveChangesAsync();
        }
    }
}