using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using moreai.web.Models;

namespace moreai.web.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException("DefaultConnection", "Connection string not found in configuration");
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Member>(
                "SELECT * FROM Members WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<Member?> GetByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Member>(
                "SELECT * FROM Members WHERE Username = @Username",
                new { Username = username });
        }

        public async Task<Member?> GetByEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Member>(
                "SELECT * FROM Members WHERE Email = @Email",
                new { Email = email });
        }

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryAsync<Member>("SELECT * FROM Members");
        }

        public async Task<int> CreateAsync(Member member)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Members (Username, Email, PasswordHash, PasswordSalt, FirstName, LastName, CreatedAt, IsActive)
                VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @FirstName, @LastName, @CreatedAt, @IsActive);
                SELECT LAST_INSERT_ID();";
            return await connection.QuerySingleAsync<int>(sql, member);
        }

        public async Task UpdateAsync(Member member)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                UPDATE Members 
                SET Email = @Email,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    LastLoginAt = @LastLoginAt,
                    IsActive = @IsActive
                WHERE Id = @Id",
                member);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "DELETE FROM Members WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<bool> ExistsAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM Members WHERE Username = @Username",
                new { Username = username });
            return count > 0;
        }

        public async Task<bool> ExistsEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM Members WHERE Email = @Email",
                new { Email = email });
            return count > 0;
        }
    }
}