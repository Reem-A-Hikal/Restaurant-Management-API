using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class RoleStrategyResolver : IRoleStrategyResolver
    {
        private readonly Dictionary<string, IRoleStrategy> _strategies;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RoleStrategyResolver(IEnumerable<IRoleStrategy> strategies,
                                    UserManager<User> userManager,
                                    IMapper mapper)
        {
            _strategies = strategies.ToDictionary(s => s.RoleName, StringComparer.OrdinalIgnoreCase);
            _userManager = userManager;
            _mapper = mapper;
        }

        public IRoleStrategy Resolve(string role)
        {
            if(string.IsNullOrWhiteSpace(role)) 
                throw new ValidationException("Role cannot be empty");
            if(_strategies.TryGetValue(role, out var strategy))
                return strategy;

            throw new BusinessException($"No strategy registered for role '{role}'." + $"Valid roles: {string.Join(",", _strategies.Keys)}");
        }

        public async Task<User> CreateUserAsync(CreateUserDto dto)
        {
            User user = dto.UserRole.ToLower() switch
            {
                "Chef" => _mapper.Map<Chef>(dto),
                "deliveryperson" => _mapper.Map<DeliveryPerson>(dto),
                _ => _mapper.Map<User>(dto)
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            var roleResult = await _userManager.AddToRoleAsync(user, dto.UserRole);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ValidationException(roleResult.Errors.Select(e => e.Description));
            }

            return user;
        }
    }
}
