using Microsoft.AspNetCore.Mvc;
using PdfSystem.Models;
using PdfSystem.Repository.IRepository;

namespace PdfSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;

        public AuthController(IAuthRepo authRepo)
        {
            _authRepo = authRepo;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Auth user)
        {
            var response = await _authRepo.Register(user);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Auth user)
        {
            var response = await _authRepo.Login(user);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
