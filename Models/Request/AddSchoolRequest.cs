using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddSchoolRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [MaxLength(20)]
    public string? CCT { get; set; }

    [Required]
    public Turns Turn { get; set; }

    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Required]
    public int EducationLevelId { get; set; }

    public int? SchoolZoneId { get; set; }
    public int? AcademySubscriptionId { get; set; }
}
