using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
{
    public class ChefStrategy : IRoleStrategy
    {
        private readonly IChefRepository _chefRepo;
        private readonly IMapper _mapper;
        public ChefStrategy(IChefRepository chefRepo, IMapper mapper)
        {
            _chefRepo = chefRepo;
            _mapper = mapper;
        }
        public string RoleName => AppRoles.Chef;

        public User CreateUserEntity(CreateUserDto dto)
            => _mapper.Map<Chef>(dto);

        public async Task EnrichDtoAsync(UserDto dto)
        {
            var chef = await _chefRepo.GetChefByIdAsync(dto.Id);
            dto.Specialization = chef?.Specialization;
        }

        public async Task UpdateRoleDataAsync(string userId, UpdateProfileDto dto)
        {
            var chef = await _chefRepo.GetChefByIdAsync(userId);
            if (chef != null && !string.IsNullOrWhiteSpace(dto.Specialization))
            {
                chef.Specialization = dto.Specialization;
                await _chefRepo.SaveChangesAsync();
            }
        }
    }
}
