using Utilities.Abstractions;

namespace Utilities.Errors;

public static class CIEErrors
{
    public static readonly Error DimensionNotFound =
        new("CIE.DimensionNotFound", "CIE dimension not found.");

    public static readonly Error EvaluationNotFound =
        new("CIE.EvaluationNotFound", "CIE evaluation not found.");

    public static readonly Error EvaluationAlreadyExistsForStudentSchoolYearAndDimension =
        new("CIE.EvaluationAlreadyExistsForStudentSchoolYearAndDimension",
            "A CIE evaluation already exists for that student, school year and dimension.");

    public static readonly Error ReviewedEvaluationCannotBeEdited =
        new("CIE.ReviewedEvaluationCannotBeEdited",
            "A reviewed CIE evaluation cannot be edited.");

    public static readonly Error SubIndicatorNotFound =
        new("CIE.SubIndicatorNotFound", "One or more CIE subindicators were not found.");

    public static readonly Error DuplicateSubIndicatorsInRequest =
        new("CIE.DuplicateSubIndicatorsInRequest",
            "The request contains duplicated subindicators.");

    public static readonly Error SubIndicatorDoesNotBelongToEvaluationDimension =
        new("CIE.SubIndicatorDoesNotBelongToEvaluationDimension",
            "One or more subindicators do not belong to the evaluation dimension.");

    public static readonly Error InvalidHelpLevel =
        new("CIE.InvalidHelpLevel",
            "Help level must be between 0 and 4.");

    public static readonly Error PhonologyAnswersRequireFonologiaDimension =
        new("CIE.PhonologyAnswersRequireFonologiaDimension",
            "Phonology answers can only be registered for the Fonología dimension.");

    public static readonly Error PhonologyAnswersRequireFonoAparatoSubIndicators =
        new("CIE.PhonologyAnswersRequireFonoAparatoSubIndicators",
            "Phonology answers can only be registered for Aparato fonoarticulador subindicators.");
}