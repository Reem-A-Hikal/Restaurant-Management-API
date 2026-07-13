using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Constants;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class CustomerStrategy : IRoleStrategy
    {
        public string RoleName => AppRoles.Customer;

        public User CreateUserEntity(CreateUserDto dto) 
            => User.Create(dto.Email, dto.UserName, dto.FullName, dto.PhoneNumber, dto.ProfileImageUrl);
        public Task EnrichDtoAsync(UserDto dto) => Task.CompletedTask;

        public Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
            => Task.CompletedTask;
    }
}
