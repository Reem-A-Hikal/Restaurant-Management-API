using Rest.Application.Dtos.UserDtos;
using Rest.Application.Utilities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(
            int pageIndex, int pageSize,
            string? searchTerm, string? selectedRole);

        Task<UserDto> GetUserByIdAsync(string userId);

        /// <summary>
        /// Creates a new user - Role is set here and CANNOT be changed later
        /// </summary>
        Task<string> AddUser(CreateUserDto userDto);

        /// <summary>
        /// Admin update - ONLY IsActive and role-specific data
        /// Role cannot be changed after creation
        /// </summary>
        Task AdminUpdateUserAsync(string userId, AdminUpdateUserDto dto);

        /// <summary>
        /// Self update - personal info only (name, phone, profile image)
        /// </summary>
        Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
        Task DeleteUser(string userId);
    }
}