using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices.StrategyFactory
{
    public interface IRoleStrategyResolver
    {
        IRoleStrategy Resolve(string role);
    }
}
