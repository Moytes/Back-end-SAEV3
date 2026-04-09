using Models.DB;

namespace Models.Dto;

public class StudentAssignmentListItemDto
{
    public Guid AssignmentStudentId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }

    public Guid MaterialId { get; set; }
    public string MaterialTitle { get; set; } = null!;
    public string? MaterialDescription { get; set; }
    public string? MaterialFileUrl { get; set; }
    public string? MaterialThumbnailUrl { get; set; }

    public DateOnly AssignedDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public assignmentStudentStatus Status { get; set; }

    public short? ProgressPercent { get; set; }
    public short? Score { get; set; }
    public string? TeacherNotes { get; set; }
    public string? StudentResponseJson { get; set; }

    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public Guid SchoolYearId { get; set; }
    public string SchoolYearName { get; set; } = null!;
}