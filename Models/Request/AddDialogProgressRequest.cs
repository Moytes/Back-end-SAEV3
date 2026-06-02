using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class AddDialogProgressRequest
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid AssignmentStudentId { get; set; }

    public short? CurrentScene { get; set; }
    public string? ChosenOption { get; set; }
    public string? EmotionDetected { get; set; }
    public string? VoiceText { get; set; }
    public BoolStatus? Completed { get; set; }
}