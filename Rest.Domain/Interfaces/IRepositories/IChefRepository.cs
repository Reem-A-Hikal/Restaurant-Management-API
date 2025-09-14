using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IChefRepository
    {
        Task<Chef?> GetChefByIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
