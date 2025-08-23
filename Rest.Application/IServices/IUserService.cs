using Rest.Application.Dtos.UserDtos;
using Rest.Application.Utilities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<PaginatedList<UserDto>> GetPaginatedUsersAsync(int pageIndex, int pageSize);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<string> AddUser(CreateUserDto userDto);
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task DeleteUser(string userId);
    }
}