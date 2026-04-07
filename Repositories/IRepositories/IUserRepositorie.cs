using Models.DB;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IUserRepositorie
{
    Task<Result<Guid>> CreateUser(AddUserRequest request, string passwordSalt, string passwordHash);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);
}
