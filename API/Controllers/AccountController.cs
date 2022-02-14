using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        //  /api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDtio) {
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
            return user;
        }

        private async Task<bool> UserExists(string username) 
        {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}