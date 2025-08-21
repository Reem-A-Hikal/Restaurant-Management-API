using Rest.Application.Dtos.UserDtos;

namespace Rest.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<string> AddUser(CreateUserDto userDto);
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task DeleteUser(string userId);
    }
}