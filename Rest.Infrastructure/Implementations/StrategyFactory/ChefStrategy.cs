using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class ChefStrategy : IRoleStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChefStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string RoleName => AppRoles.Chef;

        public User CreateUserEntity(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Specialization))
                throw new ValidationException("Specialization is required for Chef role.");

            return Chef.Create(
                dto.Email,
                dto.UserName,
                dto.FullName,
                dto.PhoneNumber,
                dto.ProfileImageUrl,
                dto.Specialization);
        }

        public async Task EnrichDtoAsync(UserDto dto)
        {
            var chef = await _unitOfWork.ChefRepository.GetChefByIdAsync(dto.Id);
            dto.Specialization = chef?.Specialization;
        }

        public async Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Specialization)) return;

            var chef = await _unitOfWork.ChefRepository.GetChefByIdAsync(userId);
            if (chef == null) return;

            chef.SetSpecialization(dto.Specialization);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
