using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class TrabajoSocialBulkRegistrationRequest
{
    [Required]
    public List<Guid> StudentIds { get; set; } = [];

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public DateOnly? IngressDate { get; set; }
    public bool IsNew { get; set; }
    public bool IsTracking { get; set; }
    public string? Notes { get; set; }
}

