using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Authentication service to the container that uses JwtBearerDefaults.AuthenticationScheme having few options.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // ValidateIssuerSigningKey is used to validate the signature of the token.
                    ValidateIssuerSigningKey = true,

                    // IssuerSigningKey store the key that is used to sign the token.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),

                    ValidateIssuer = false,

                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}