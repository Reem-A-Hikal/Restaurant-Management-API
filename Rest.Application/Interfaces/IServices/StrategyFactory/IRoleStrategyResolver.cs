namespace Rest.Application.Interfaces.IServices.StrategyFactory
{
    public interface IRoleStrategyResolver
    {
        IRoleStrategy Resolve(string role);
    }
}
