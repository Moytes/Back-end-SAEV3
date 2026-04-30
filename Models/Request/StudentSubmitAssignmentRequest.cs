using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class StudentSubmitAssignmentRequest
{
    [Required]
    public string StudentResponseJson { get; set; } = null!;
}
