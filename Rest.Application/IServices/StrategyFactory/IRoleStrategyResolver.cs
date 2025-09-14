

namespace Rest.Application.IServices.StrategyFactory
{
    public interface IRoleStrategyResolver
    {
        IRoleStrategy Resolve(string role);
    }
}
