using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
{
    public class CustomerStrategy : IRoleStrategy
    {
        private readonly IMapper _mapper;
        public CustomerStrategy(IMapper mapper)
        {
            _mapper = mapper;
        }

        public string RoleName => AppRoles.Customer;

        public User CreateUserEntity(CreateUserDto dto)
            => _mapper.Map<User>(dto);

        public Task EnrichDtoAsync(UserDto dto) => Task.CompletedTask;

        public Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
            => Task.CompletedTask;
    }
}
