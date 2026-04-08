using Utilities.Abstractions;

namespace Utilities.Errors;

public static class AttentionModeErrors
{
    public static readonly Error AttentionModeAlreadyExists =
        new("AttentionMode.AttentionModeAlreadyExists",
            "That attention mode is already registered for the student in the selected school year.");
}