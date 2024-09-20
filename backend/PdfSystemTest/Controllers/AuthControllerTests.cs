using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PdfSystem.Controllers;
using PdfSystem.Models;
using PdfSystem.Repository.IRepository;
using PdfSystem.Services;

namespace PdfSystemTest.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthRepo> _mockAuthRepo;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthRepo = new Mock<IAuthRepo>();
            _mockTokenService = new Mock<ITokenService>();
            _authController = new AuthController(_mockAuthRepo.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            var fakeUser = new Auth { Email = "test@test.com", Password = "SuperTest123*" };

            _mockAuthRepo.Setup(repo => repo.Register(It.IsAny<Auth>()))
                .ReturnsAsync(new ResponseAPI
                {
                    Success = true,
                    Msg = "User created successfully"
                });

            var result = await _authController.Register(fakeUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("User created successfully", response.Msg);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            var fakeUser = new Auth { Email = "existing@test.com", Password = "SuperTest123*" };

            _mockAuthRepo.Setup(repo => repo.Register(It.IsAny<Auth>()))
                .ReturnsAsync(new ResponseAPI
                {
                    Success = false,
                    Msg = "User already exists",
                    Data = null
                });

            var result = await _authController.Register(fakeUser);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(badRequestResult.Value);


            Assert.False(response.Success);
            Assert.Equal("User already exists", response.Msg);
        }


        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {

            var fakeUser = new Auth { Email = "test@test.com", Password = "SuperTest123*" };
            var token = "fake-jwt-token";


            _mockAuthRepo.Setup(repo => repo.Login(It.IsAny<Auth>()))
                .ReturnsAsync(new ResponseAPI
                {
                    Success = true,
                    Msg = "Login successful",
                    Data = new { Token = token }
                });

            _mockTokenService.Setup(service => service.GenerateToken(It.IsAny<IdentityUser>()))
                .Returns(token);


            var result = await _authController.Login(fakeUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(okResult.Value);

            Assert.True(response.Success);
            Assert.Equal("Login successful", response.Msg);
            Assert.Equal(token, ((dynamic)response.Data).Token);
        }


        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            var fakeUser = new Auth { Email = "nonexistent@test.com", Password = "SuperTest123*" };

            _mockAuthRepo.Setup(repo => repo.Login(It.IsAny<Auth>()))
                .ReturnsAsync(new ResponseAPI
                {
                    Success = false,
                    Msg = "Invalid Credentials",
                    Data = null
                });

            var result = await _authController.Login(fakeUser);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseAPI>(badRequestResult.Value);

            Assert.False(response.Success);
            Assert.Equal("Invalid Credentials", response.Msg);
        }
    }

}
