using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        // POST api/account/register
        // This method is used to register a new user.
        [HttpPost("register")]
        public async Task<ActionResult<UserTokenDto>> Register(RegisterDto registerDto)
        {

            // Check if the user already exists.
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            // For Hashing the password we would use crytographic algorith HMACSHA512.
            // using statement is used to dispose the object after it is used. 
            // As HMACSHA512 implements IDisposable interface, it can be used in using statement.
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                Name = registerDto.Name,
                DonorType = registerDto.DonorType,
                VolunteerType = registerDto.VolunteerType,
                AddressLine1 = registerDto.AddressLine1,
                AddressLine2 = registerDto.AddressLine2,
                City = registerDto.City,
                Pincode = registerDto.Pincode,
                PhoneNumber = registerDto.PhoneNumber,
                PhotoUrl = registerDto.PhotoUrl
            };

            // Add the user to the database.
            _context.Users.Add(user);

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // return the UserTokenDto
            return new UserTokenDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        // POST api/account/login
        // This method is used to login a user.
        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDto>> Login(LoginDto loginDto)
        {

            // Get the user from the database.
            // FirstOrDefaultAsync() method is used to get the first element of a sequence or a default value if the sequence contains no elements.
            // SingleOrDefaultAsync() method is used to get the only element of a sequence, or a default value if the sequence is empty; 
            // this method throws an exception if there is more than one element in the sequence.

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

            // if user is not found, return Unauthorized.
            if (user == null) return Unauthorized("Invalid Username");

            // if user exists then check the password.

            // getting hmac object that uses key as user.PasswordSalt.
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // if the computed hash is not equal to the user.PasswordHash, return Unauthorized.
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            
            // if the user is found and password is correct, return the UserTokenDto.
            return new UserTokenDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("updatePassword")]
        public async Task<ActionResult<UserTokenDto>> updatePassword(ChangePasswordDto cpd)
        {
            // Get the user from the database.
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == cpd.Username);

            // if user is not found, return Unauthorized.
            if (user == null) return Unauthorized("Invalid Username");

            // if user exists then check the password.
            // getting hmac object that uses key as user.PasswordSalt.
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(cpd.OldPassword));

            // if the computed hash is not equal to the user.PasswordHash, return Unauthorized.
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(cpd.NewPassword));

            // if the user is found and password is correct, return the UserTokenDto.
             _context.Entry(user).State = EntityState.Modified;

            if(await _context.SaveChangesAsync() > 0)
            {
                return new UserTokenDto
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }
            return BadRequest("Failed to change password!");
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}