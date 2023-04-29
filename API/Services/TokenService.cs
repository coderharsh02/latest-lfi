using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {   
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            // Create a list of claims.
            var claims = new List<Claim>
            {
                // Add the user id to the claims.
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // creds is used to sign the token.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // tokenDescriptor is used to create the token.
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                // Subject is used to add the claims to the token.
                Subject = new ClaimsIdentity(claims),

                // Expires is used to set the expiry date of the token.
                Expires = DateTime.Now.AddDays(7),

                // SigningCredentials is used to sign the token.
                SigningCredentials = creds
            };

            // Create a token handler.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create the token using the token handler.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write the token to a string and return it.
            return tokenHandler.WriteToken(token);
        }
    }
}