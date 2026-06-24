using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class TrabajoSocialQuickStudentRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FatherLastName { get; set; } = null!;

    [MaxLength(100)]
    public string? MotherLastName { get; set; }

    [Required]
    public Sexo Sexo { get; set; }

    [Required]
    public DateOnly BirthDate { get; set; }

    [StringLength(18)]
    public string? CURP { get; set; }

    public string? PhotoUrl { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int SchoolYearId { get; set; }

    public DateOnly? IngressDate { get; set; }
    public bool IsNew { get; set; } = true;
    public bool IsTracking { get; set; }
    public FinalSituation? FinalSituation { get; set; }
    public string? Notes { get; set; }

    [MaxLength(200)]
    public string? TutorCompleteName { get; set; }

    [MaxLength(50)]
    public string? TutorParentesco { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? TutorPhone { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? TutorEmail { get; set; }

    public string? TutorAddress { get; set; }
    public bool CreateTutorAccount { get; set; }

    [MinLength(8)]
    [MaxLength(100)]
    public string? TutorPassword { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string? StudentEmail { get; set; }

    [MinLength(8)]
    [MaxLength(100)]
    public string? StudentPassword { get; set; }
}
