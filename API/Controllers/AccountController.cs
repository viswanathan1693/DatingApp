using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username)) return BadRequest("Username already exists..!");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO() { Username = user.UserName, Token = _tokenService.CreateToken(user) };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users
            .AnyAsync(x => x.UserName.Equals(username.ToLower()));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

            if (user == null) return Unauthorized("Invalid Username and password");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash)) return Unauthorized("Invalid password");

            return new UserDTO() { Username = user.UserName, Token = _tokenService.CreateToken(user), PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url };

        }

    }
}