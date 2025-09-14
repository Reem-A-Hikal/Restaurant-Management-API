using Rest.Application.IServices.StrategyFactory;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
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
            return _strategies.TryGetValue(role, out var strategy)
                ? strategy
                : throw new KeyNotFoundException($"No strategy registered for role '{role}'.");
        }
    }
}
