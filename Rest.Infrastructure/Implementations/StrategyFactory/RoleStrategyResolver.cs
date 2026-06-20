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

        public RoleStrategyResolver(IEnumerable<IRoleStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.RoleName, StringComparer.OrdinalIgnoreCase);
        }

        public IRoleStrategy Resolve(string role)
        {
            if(string.IsNullOrWhiteSpace(role)) 
                throw new ValidationException("Role cannot be empty");
            if(_strategies.TryGetValue(role, out var strategy))
                return strategy;

            throw new BusinessException($"No strategy registered for role '{role}'." + $"Valid roles: {string.Join(",", _strategies.Keys)}");
        }
    }
}
