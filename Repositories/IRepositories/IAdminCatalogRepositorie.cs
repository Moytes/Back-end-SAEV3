using Models.DB;
using Models.Dto;
using Models.Request;
using Utilities.Abstractions;

namespace Repositories.IRepositories;

public interface IAdminCatalogRepositorie
{
    Task<IEnumerable<SchoolYearListItemDto>> GetSchoolYears(bool? onlyActive = null);
    Task<Result<int>> CreateSchoolYear(AddSchoolYearRequest request);

    Task<IEnumerable<SchoolZoneListItemDto>> GetSchoolZones();
    Task<Result<int>> CreateSchoolZone(AddSchoolZoneRequest request);

    Task<IEnumerable<SchoolListItemDto>> GetSchools(int? schoolZoneId = null);
    Task<Result<int>> CreateSchool(AddSchoolRequest request);

    Task<IEnumerable<GroupListItemDto>> GetGroups(int? schoolId = null, int? schoolYearId = null);
    Task<IEnumerable<GroupWithTeachersDto>> GetGroupsWithTeachers(int? schoolId = null, int? schoolYearId = null);
    Task<Result<int>> CreateGroup(AddGroupRequest request);

    Task<IEnumerable<EducationLevel>> GetEducationLevels();
    Task<IEnumerable<Grade>> GetGrades(int? educationLevelId = null);
    Task<IEnumerable<Role>> GetRoles();
}
