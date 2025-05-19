using Rest.API.Dtos.UserDtos;

namespace Rest.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task DeleteUser(string userId);
    }
}