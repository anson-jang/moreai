using System.Collections.Generic;
using System.Threading.Tasks;
using moreai.web.Models;

namespace moreai.web.Repositories
{
    public interface IGroupRepository
    {
        Task<Group> GetByIdAsync(int id);
        Task<IEnumerable<Group>> GetAllAsync();
        Task<int> CreateAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string name);
        Task<IEnumerable<Group>> GetMemberGroupsAsync(int memberId);
        Task AddMemberToGroupAsync(int memberId, int groupId);
        Task RemoveMemberFromGroupAsync(int memberId, int groupId);
    }
}