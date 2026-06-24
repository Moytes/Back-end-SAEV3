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

    Task<IEnumerable<UserListItemDto>> GetUsers(int? roleId, int? schoolZoneId, int? schoolId);
    Task<Result<bool>> UpdateUser(Guid userId, UpdateUserRequest request);
    Task<bool> UserExists(Guid userId);
    Task<bool> EmailExists(string email, Guid? excludeUserId = null);
    Task<Result<int>> AssignUserToGroup(Guid userId, AssignUserGroupRequest request);
    Task<Result<int>> AssignUserToSchool(Guid userId, AssignUserSchoolRequest request);
    Task<Result<int>> AssignSupervisorToSchool(Guid userId, AssignSupervisorSchoolRequest request);
    Task<IEnumerable<int>> GetUserSchools(Guid userId);
    Task<IEnumerable<UserListItemDto>> GetDocentesBySchool(int schoolId);
    Task<IEnumerable<SchoolListItemDto>> GetSupervisorSchools(Guid supervisorId);
    Task<IEnumerable<UserListItemDto>> GetSupervisorStaff(Guid supervisorId, int? roleId, int? schoolId);
    Task<Result<Guid>> CreateSupervisorStaff(Guid supervisorId, SupervisorCreateStaffRequest request, string passwordSalt, string passwordHash);
    Task<Result<bool>> UpdateSupervisorStaff(Guid supervisorId, Guid staffId, SupervisorUpdateStaffRequest request);
}
