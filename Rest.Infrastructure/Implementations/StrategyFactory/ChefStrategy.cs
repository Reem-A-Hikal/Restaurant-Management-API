using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.StrategyFactory
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

        public async Task EnrichDtoAsync(UserDto dto)
        {
            var chef = await _chefRepo.GetChefByIdAsync(dto.Id);
            dto.Specialization = chef?.Specialization;
        }

        public async Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Specialization))
            {
                var chef = await _chefRepo.GetChefByIdAsync(userId);
                if (chef == null) return;
                
                chef.Specialization = dto.Specialization;
                await _chefRepo.SaveChangesAsync();
                
            }
        }
    }
}
