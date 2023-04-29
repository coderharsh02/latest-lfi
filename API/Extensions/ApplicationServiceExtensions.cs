using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add DbContext service to the container and configure it to use Sqlite so that it can be used thoroughout the program. 
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // Add Cors service to the container.
            services.AddCors();

            // creating interface for service is useful when we want to test the service and is a good practice.
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRepository, Repository>();
            return services;
        }
    }
}