using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices.StrategyFactory
{
    public interface IRoleStrategy
    {
        string RoleName { get; }
        User CreateUserEntity(CreateUserDto dto);
        Task EnrichDtoAsync(UserDto dto);
        Task UpdateRoleDataAsync(string userId, UpdateProfileDto dto);
    }
}
