using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class CustomerStrategy : IRoleStrategy
    {
        public string RoleName => AppRoles.Customer;

        public Task EnrichDtoAsync(UserDto dto) => Task.CompletedTask;

        public Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
            => Task.CompletedTask;
    }
}
