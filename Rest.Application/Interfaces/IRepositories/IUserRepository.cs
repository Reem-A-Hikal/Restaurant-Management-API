using Rest.Application.Dtos.UserDtos;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<PaginatedList<User>> GetPaginatedAsync( int pageIndex, int pageSize, string? searchTerm, string? selectedRole);
        Task<User> GetByIdAsync(string id);
        Task<Dictionary<string, string>> GetUsersRolesDictAsync(IEnumerable<string> userIds);
        Task BulkEnrichUsersAsync(List<UserDto> userDtos);
        Task AddAsync(User entity);
        void Update(User entity);
        Task SaveChangesAsync();
    }
}
