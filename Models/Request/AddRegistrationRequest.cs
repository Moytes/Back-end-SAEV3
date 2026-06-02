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
    public BoolStatus ItsNew { get; set; } = BoolStatus.False;
    public BoolStatus ItsTracking { get; set; } = BoolStatus.False;
    public finalSituation FinalSituation { get; set; } = finalSituation.SEGUIMIENTO;
    public string? Notes { get; set; }
}