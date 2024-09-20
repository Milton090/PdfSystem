using Microsoft.AspNetCore.Identity;
using PdfSystem.Data;
using PdfSystem.Models;
using PdfSystem.Repository.IRepository;
using PdfSystem.Services;

namespace PdfSystem.Repository
{
    public class AuthRepo : IAuthRepo
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthRepo(AppDbContext db, UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            _db = db;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<ResponseAPI> Register(Auth user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "User already exists",
                    Data = null
                };
            }

            var identityUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (!result.Succeeded)
            {
                return new ResponseAPI { 
                    Success = false, 
                    Msg = String.Join(", ", result.Errors.Select(e => e.Description)),
                    Data = null
                };
            }

            return new ResponseAPI
            {
                Success = true,
                Msg = "User created successfully",
                Data = null
            };
        }

        public async Task<ResponseAPI> Login(Auth user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "Invalid Credentials",
                    Data = null
                };
            }

            var result = await _userManager.CheckPasswordAsync(identityUser, user.Password);
            if (!result)
            {
                return new ResponseAPI
                {
                    Success = false,
                    Msg = "Invalid Credentials",
                    Data = null
                };
            }

            var token = _tokenService.GenerateToken(identityUser);

            return new ResponseAPI
            {
                Success = true,
                Msg = "Login successful",
                Data = new { Token = token }
            };
        }

    }
}
