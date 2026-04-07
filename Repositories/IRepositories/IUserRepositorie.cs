using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IUserRepositorie
{
    Task<Result<Guid>> CreateUser(AddUserRequest request, string passwordSalt, string passwordHash);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserById(Guid userId);

    Task<IEnumerable<UserListItemDto>> GetUsers(UserRole? role, Guid? schoolZoneId, Guid? schoolId);
    Task<Result<bool>> UpdateUser(Guid userId, UpdateUserRequest request);
    Task<bool> UserExists(Guid userId);
    Task<bool> EmailExists(string email, Guid? excludeUserId = null);
    Task<Result<Guid>> AssignUserToGroup(Guid userId, AssignUserGroupRequest request);
    Task<Result<Guid>> AssignUserToSchool(Guid userId, AssignUserSchoolRequest request);
}