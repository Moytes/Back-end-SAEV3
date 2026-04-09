using Models.Dto;

namespace Repositories.IRepositories;

public interface IReportRepositorie
{
    Task<IEnumerable<StudentDataSheetItemDto>> GetStudentDataSheet(Guid? schoolId, Guid? schoolYearId);
    Task<IEnumerable<CIESummaryItemDto>> GetCIESummary(Guid? studentId, Guid? schoolYearId);
    Task<IEnumerable<TEAAlertItemDto>> GetTEAAlerts(Guid? schoolYearId, Models.DB.alertLevel? alertLevel);
}