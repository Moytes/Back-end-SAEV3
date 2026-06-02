using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddStudentRequest
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
    public Gender Gender { get; set; }

    [Required]
    public DateOnly BirthDate { get; set; }

    [StringLength(18)]
    public string? CURP { get; set; }

    public string? PhotoUrl { get; set; }
}