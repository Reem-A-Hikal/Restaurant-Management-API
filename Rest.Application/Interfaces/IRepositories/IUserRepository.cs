using Rest.Application.Dtos.UserDtos;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        Task<PaginatedList<User>> GetPaginatedAsync( int pageIndex, int pageSize, string? searchTerm, string? selectedRole);
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
        Task<Dictionary<string, string>> GetUsersRolesDictAsync(IEnumerable<string> userIds);
        Task BulkEnrichUsersAsync(List<UserDto> userDtos);
    }
}
