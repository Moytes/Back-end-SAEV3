using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddRegistrationRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public DateOnly? IngressDate { get; set; }
    public bool IsNew { get; set; }
    public bool IsTracking { get; set; }
    public FinalSituation? FinalSituation { get; set; }
    public string? Notes { get; set; }
}
