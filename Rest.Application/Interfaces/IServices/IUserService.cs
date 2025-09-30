using Rest.Application.Dtos.UserDtos;
using Rest.Application.Utilities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedRole);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<string> AddUser(CreateUserDto userDto);
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task DeleteUser(string userId);
    }
}