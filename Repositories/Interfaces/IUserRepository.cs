using Rest.API.Models;

namespace Rest.API.Repositories.Interfaces
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
    }
}
