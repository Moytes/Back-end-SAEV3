using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class AddSchoolZoneRequest
{
    [Required]
    [MaxLength(10)]
    public string Number { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string CCT { get; set; } = null!;

    [MaxLength(200)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? AcademySubscriptionId { get; set; }
}
