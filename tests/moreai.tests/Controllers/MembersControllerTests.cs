using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using moreai.web.Controllers;
using moreai.web.Models;
using moreai.web.Services;

namespace moreai.tests.Controllers
{
    public class MembersControllerTests
    {
        private readonly Mock<IMemberService> _mockService;
        private readonly MembersController _controller;

        public MembersControllerTests()
        {
            _mockService = new Mock<IMemberService>();
            _controller = new MembersController(_mockService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var request = new RegisterRequest 
            { 
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            _mockService.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(new ServiceResult<int> { Success = true, Data = 1 });

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResult<int>>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
        {
            // Arrange
            var request = new LoginRequest 
            { 
                Email = "test@example.com",
                Password = "password123"
            };

            var loginResponse = new LoginResponse
            {
                Token = "jwt_token",
                Username = "testuser"
            };

            _mockService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(new ServiceResult<LoginResponse> { Success = true, Data = loginResponse });

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResult<LoginResponse>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(loginResponse.Token, response.Data.Token);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnOk_WhenProfileExists()
        {
            // Arrange
            int memberId = 1;
            var profile = new MemberProfile
            {
                Username = "testuser",
                Email = "test@example.com"
            };

            _mockService.Setup(s => s.GetProfileAsync(memberId))
                .ReturnsAsync(new ServiceResult<MemberProfile> { Success = true, Data = profile });

            // Act
            var result = await _controller.GetProfile(memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResult<MemberProfile>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(profile.Username, response.Data.Username);
        }
    }
}