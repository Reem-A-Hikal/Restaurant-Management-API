using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
    }
}
