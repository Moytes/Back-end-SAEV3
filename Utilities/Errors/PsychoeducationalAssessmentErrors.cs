using Utilities.Abstractions;

namespace Utilities.Errors;

public static class PsychoeducationalAssessmentErrors
{
    public static readonly Error AssessmentNotFound =
        new("PsychoeducationalAssessment.AssessmentNotFound", "Psychoeducational assessment not found.");

    public static readonly Error AssessmentAlreadyExistsForStudentAndSchoolYear =
        new("PsychoeducationalAssessment.AssessmentAlreadyExistsForStudentAndSchoolYear",
            "A psychoeducational assessment already exists for that student and school year.");

    public static readonly Error DeliveredAssessmentCannotBeEdited =
        new("PsychoeducationalAssessment.DeliveredAssessmentCannotBeEdited",
            "A delivered psychoeducational assessment cannot be edited.");

    public static readonly Error ExternalCollaboratorNameRequired =
        new("PsychoeducationalAssessment.ExternalCollaboratorNameRequired",
            "External collaborators must include a name.");
}