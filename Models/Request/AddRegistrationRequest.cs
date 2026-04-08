using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddRegistrationRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid SchoolYearId { get; set; }

    public DateOnly? IngressDate { get; set; }
    public boolStatus ItsNew { get; set; } = boolStatus.False;
    public boolStatus ItsTracking { get; set; } = boolStatus.False;
    public finalSituation FinalSituation { get; set; } = finalSituation.SEGUIMIENTO;
    public string? Notes { get; set; }
}