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
        private readonly ITokenService _tokenSerivce;
        public AccountController(DataContext context, ITokenService tokenSerivce)
        {
            _tokenSerivce = tokenSerivce;
            _context = context;
        }

        //  /api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDtio) {
            if (await UserExists(registerDtio.UserName)) return BadRequest("Username already exists");
            using var hmac = new HMACSHA512();      //hashing algorithm
            //using: when we finish using the class, it will dispose correctly
            var user = new AppUser {
                UserName = registerDtio.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDtio.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return new UserDto {
                UserName = user.UserName,
                Token = _tokenSerivce.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDtio) {
            var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == loginDtio.UserName);

            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtio.Password));

            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto {
                UserName = user.UserName,
                Token = _tokenSerivce.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username) 
        {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}