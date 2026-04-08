using Utilities.Abstractions;

namespace Utilities.Errors;

public static class DisabilityErrors
{
    public static readonly Error DisabilityNotFound =
        new("Disability.DisabilityNotFound", "Disability catalog item not found.");

    public static readonly Error StudentDisabilityAlreadyExists =
        new("Disability.StudentDisabilityAlreadyExists",
            "That disability or BAP is already assigned to the student for the selected school year.");
}