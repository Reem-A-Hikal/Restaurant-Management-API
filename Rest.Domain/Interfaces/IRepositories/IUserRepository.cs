using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        IQueryable<User> GetAllQueryable();
        IQueryable<User> GetFilteredUsers(string? searchTerm, string? selectedRole = "All");
        Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
    }
}
