using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddSchoolRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string CCT { get; set; } = null!;

    [Required]
    public Turns Turn { get; set; }

    public string? Address { get; set; }

    [Required]
    public Guid SchoolZoneId { get; set; }
}
