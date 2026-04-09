using System.ComponentModel.DataAnnotations;

namespace Models.Request;

public class CompleteAssignmentStudentRequest
{
    [Range(0, 100)]
    public short? ProgressPercent { get; set; }

    [Range(0, 100)]
    public short? Score { get; set; }

    public string? TeacherNotes { get; set; }
    public string? StudentResponseJson { get; set; }
}