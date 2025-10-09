using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using moreai.web.Models;

namespace moreai.web.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly string _connectionString;

        public GroupRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Group> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Group>(
                "SELECT * FROM Groups WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryAsync<Group>("SELECT * FROM Groups");
        }

        public async Task<int> CreateAsync(Group group)
        {
            using var connection = new MySqlConnection(_connectionString);
            string sql = @"
                INSERT INTO Groups (Name, Description, CreatedAt, IsActive)
                VALUES (@Name, @Description, @CreatedAt, @IsActive);
                SELECT LAST_INSERT_ID();";
            return await connection.QuerySingleAsync<int>(sql, group);
        }

        public async Task UpdateAsync(Group group)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                UPDATE Groups 
                SET Name = @Name,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE Id = @Id",
                group);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "DELETE FROM Groups WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<bool> ExistsAsync(string name)
        {
            using var connection = new MySqlConnection(_connectionString);
            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM Groups WHERE Name = @Name",
                new { Name = name });
            return count > 0;
        }

        public async Task<IEnumerable<Group>> GetMemberGroupsAsync(int memberId)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryAsync<Group>(@"
                SELECT g.* 
                FROM Groups g
                INNER JOIN MemberGroups mg ON g.Id = mg.GroupId
                WHERE mg.MemberId = @MemberId",
                new { MemberId = memberId });
        }

        public async Task AddMemberToGroupAsync(int memberId, int groupId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                INSERT INTO MemberGroups (MemberId, GroupId, JoinedAt)
                VALUES (@MemberId, @GroupId, @JoinedAt)",
                new { MemberId = memberId, GroupId = groupId, JoinedAt = DateTime.UtcNow });
        }

        public async Task RemoveMemberFromGroupAsync(int memberId, int groupId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
                DELETE FROM MemberGroups 
                WHERE MemberId = @MemberId AND GroupId = @GroupId",
                new { MemberId = memberId, GroupId = groupId });
        }
    }
}