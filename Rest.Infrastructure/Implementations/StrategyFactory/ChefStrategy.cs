using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Constants;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class ChefStrategy : IRoleStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ChefStrategy(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public string RoleName => AppRoles.Chef;

        public User CreateUserEntity(CreateUserDto dto) => _mapper.Map<Chef>(dto);

        public async Task EnrichDtoAsync(UserDto dto)
        {
            var chef = await _unitOfWork.ChefRepository.GetChefByIdAsync(dto.Id);
            dto.Specialization = chef?.Specialization;
        }

        public async Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Specialization))
            {
                var chef = await _unitOfWork.ChefRepository.GetChefByIdAsync(userId);
                if (chef == null) return;
                
                chef.Specialization = dto.Specialization;
                await _unitOfWork.SaveChangesAsync();
                
            }
        }
    }
}
