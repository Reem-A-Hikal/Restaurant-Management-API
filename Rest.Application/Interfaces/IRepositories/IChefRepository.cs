using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IChefRepository
    {
        Task<Chef?> GetChefByIdAsync(string userId);
        Task BulkEnrichChefsAsync(List<UserDto> userDtos);
    }
}
