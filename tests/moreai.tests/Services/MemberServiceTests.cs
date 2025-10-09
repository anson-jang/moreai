using Xunit;
using Moq;
using moreai.web.Models;
using moreai.web.Services;
using moreai.web.Repositories;

namespace moreai.tests.Services
{
    public class MemberServiceTests
    {
        private readonly Mock<IMemberRepository> _mockRepository;
        private readonly MemberService _service;

        public MemberServiceTests()
        {
            _mockRepository = new Mock<IMemberRepository>();
            _service = new MemberService(_mockRepository.Object);
        }

        [Fact]
        public async Task RegisterMember_ShouldSucceed_WithValidInput()
        {
            // Arrange
            var registerRequest = new RegisterRequest 
            { 
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Member>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.RegisterAsync(registerRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task LoginMember_ShouldSucceed_WithValidCredentials()
        {
            // Arrange
            var loginRequest = new LoginRequest 
            { 
                Email = "test@example.com",
                Password = "password123"
            };

            var storedMember = new Member
            {
                Id = 1,
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Salt = "salt"
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(loginRequest.Email))
                .ReturnsAsync(storedMember);

            // Act
            var result = await _service.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetMemberProfile_ShouldReturnProfile_WhenMemberExists()
        {
            // Arrange
            int memberId = 1;
            var expectedMember = new Member 
            { 
                Id = memberId,
                Username = "testuser",
                Email = "test@example.com"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(memberId))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _service.GetProfileAsync(memberId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedMember.Username, result.Data.Username);
        }
    }
}