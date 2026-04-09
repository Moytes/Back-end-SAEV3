using System.ComponentModel.DataAnnotations;
using Models.DB;

namespace Models.Request;

public class UpdatePsychoeducationalAssessmentRequest
{
    public DateOnly? EvaluationDate { get; set; }

    public string? EvaluationReason { get; set; }
    public string? EvaluationBehavior { get; set; }

    public string? PregnancyHistory { get; set; }
    public string? HereditaryHistory { get; set; }
    public string? MotorDevelopment { get; set; }
    public string? LanguageDevelopment { get; set; }
    public string? MedicalHistory { get; set; }
    public string? SchoolHistory { get; set; }
    public string? FamilySituation { get; set; }

    public string? StudentDescription { get; set; }
    public string? FamilyContext { get; set; }
    public string? SchoolContext { get; set; }
    public string? SocialContext { get; set; }
    public string? PhysicalDevelopment { get; set; }
    public string? CognitiveDevelopment { get; set; }
    public string? SocioAffectiveDevelopment { get; set; }
    public string? LearningEvaluation { get; set; }
    public string? Creativity { get; set; }

    public string? ResultsInterpretation { get; set; }
    public string? Conclusions { get; set; }

    public assessmentStatus? Status { get; set; }
}