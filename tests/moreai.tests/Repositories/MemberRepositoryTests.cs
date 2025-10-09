using Xunit;
using Moq;
using moreai.web.Models;
using moreai.web.Repositories;
using MySqlConnector;
using Dapper;

namespace moreai.tests.Repositories
{
    public class MemberRepositoryTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly MemberRepository _repository;

        public MemberRepositoryTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            _repository = new MemberRepository(_mockConnection.Object);
        }

        [Fact]
        public async Task GetMemberById_ShouldReturnMember_WhenMemberExists()
        {
            // Arrange
            var expectedMember = new Member 
            { 
                Id = 1, 
                Username = "testuser",
                Email = "test@example.com"
            };

            _mockConnection.Setup(c => c.QueryFirstOrDefaultAsync<Member>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null
            )).ReturnsAsync(expectedMember);

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMember.Id, result.Id);
            Assert.Equal(expectedMember.Username, result.Username);
        }

        [Fact]
        public async Task CreateMember_ShouldReturnNewId_WhenSuccessful()
        {
            // Arrange
            var member = new Member 
            { 
                Username = "newuser",
                Email = "new@example.com",
                PasswordHash = "hashedpassword",
                Salt = "salt"
            };

            _mockConnection.Setup(c => c.ExecuteScalarAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null
            )).ReturnsAsync(1);

            // Act
            var result = await _repository.CreateAsync(member);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UpdateMember_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var member = new Member 
            { 
                Id = 1,
                Username = "updateduser",
                Email = "updated@example.com"
            };

            _mockConnection.Setup(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null
            )).ReturnsAsync(1);

            // Act
            var result = await _repository.UpdateAsync(member);

            // Assert
            Assert.True(result);
        }
    }
}