using Utilities.Abstractions;

namespace Utilities.Errors;

public static class DialogErrors
{
    public static readonly Error MaterialMustBeDialogType =
        new("Dialog.MaterialMustBeDialogType",
            "The selected material must have material type DIALOGO_ANIMADO.");

    public static readonly Error DialogNotFound =
        new("Dialog.DialogNotFound", "Dialog not found.");

    public static readonly Error AssignmentStudentDoesNotBelongToStudent =
        new("Dialog.AssignmentStudentDoesNotBelongToStudent",
            "The assignment-student record does not belong to the provided student.");

    public static readonly Error DialogDoesNotBelongToAssignmentMaterial =
        new("Dialog.DialogDoesNotBelongToAssignmentMaterial",
            "The dialog does not belong to the material assigned in that assignment.");
}