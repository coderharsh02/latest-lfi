using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // To use this class we need to register it in Program.cs
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // AppUser is how we refer to user in our code but database would create a table called Users same as DbSet name.
        // _context.Users refers to this DbSet.
        public DbSet<AppUser> Users { get; set; }

        // Donation is how we refer to donation in our code but database would create a table called Donations same as DbSet name.
        // _context.Donations refers to this DbSet.
        public DbSet<Donation> Donations { get; set; }
    }
}