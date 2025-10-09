using System.Collections.Generic;
using System.Threading.Tasks;
using moreai.web.Models;

namespace moreai.web.Repositories
{
    public interface IMemberRepository
    {
        Task<Member?> GetByIdAsync(int id);
        Task<Member?> GetByUsernameAsync(string username);
        Task<Member?> GetByEmailAsync(string email);
        Task<IEnumerable<Member>> GetAllAsync();
        Task<int> CreateAsync(Member member);
        Task UpdateAsync(Member member);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string username);
        Task<bool> ExistsEmailAsync(string email);
    }
}