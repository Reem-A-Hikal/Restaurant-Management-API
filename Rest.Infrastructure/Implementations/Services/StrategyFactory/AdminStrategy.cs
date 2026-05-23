using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
{
    public class AdminStrategy : IRoleStrategy
    {
        private readonly IMapper _mapper;
        public AdminStrategy(IMapper mapper)
        {
            _mapper = mapper;
        }

        public string RoleName => AppRoles.Admin;

        public User CreateUserEntity(CreateUserDto dto)
            => _mapper.Map<User>(dto);

        public Task EnrichDtoAsync(UserDto dto) => Task.CompletedTask;

        public Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
            => Task.CompletedTask;
    }
}
