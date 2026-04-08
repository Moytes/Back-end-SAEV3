using Models.Dto;

namespace Repositories.IRepositories;

public interface IAdminCatalogRepositorie
{
    Task<IEnumerable<SchoolYearListItemDto>> GetSchoolYears(bool? onlyActive = null);
    Task<IEnumerable<SchoolZoneListItemDto>> GetSchoolZones();
    Task<IEnumerable<SchoolListItemDto>> GetSchools(Guid? schoolZoneId = null);
    Task<IEnumerable<GroupListItemDto>> GetGroups(Guid? schoolId = null, Guid? schoolYearId = null);
}