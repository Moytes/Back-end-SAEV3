using Utilities.Abstractions;

namespace Utilities.Errors;

public static class CanalizationErrors
{
    public static readonly Error CanalizationNotFound =
        new("Canalization.CanalizationNotFound", "Canalization not found.");

    public static readonly Error RequesterAndReceiverMustBeDifferent =
        new("Canalization.RequesterAndReceiverMustBeDifferent",
            "Requester and receiver must be different users.");

    public static readonly Error ClosedCanalizationCannotBeReopened =
        new("Canalization.ClosedCanalizationCannotBeReopened",
            "A closed canalization cannot be reopened.");
}