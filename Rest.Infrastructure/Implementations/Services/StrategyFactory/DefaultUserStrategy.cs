using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
{
    public class DefaultUserStrategy : IRoleStrategy
    {
        private readonly IMapper _mapper;
        private readonly string _roleName;
        public DefaultUserStrategy(IMapper mapper, string roleName)
        {
            _mapper = mapper;
            _roleName = roleName;
        }

        public string RoleName => _roleName;

        public User CreateUserEntity(CreateUserDto dto)
            => _mapper.Map<User>(dto);

        public Task EnrichDtoAsync(UserDto dto) => Task.CompletedTask;

        public Task UpdateRoleDataAsync(string userId, UpdateProfileDto dto)
            => Task.CompletedTask;
    }
}
