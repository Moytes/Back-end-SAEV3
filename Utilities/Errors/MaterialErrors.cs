using Utilities.Abstractions;

namespace Utilities.Errors;

public static class MaterialErrors
{
    public static readonly Error MaterialNotFound =
        new("Material.MaterialNotFound", "Material not found.");

    public static readonly Error MaterialTypeNotFound =
        new("Material.MaterialTypeNotFound", "Material type not found.");

    public static readonly Error InvalidGradeRange =
        new("Material.InvalidGradeRange", "GradeMin cannot be greater than GradeMax.");

    public static readonly Error EmptyTagsAreNotAllowed =
        new("Material.EmptyTagsAreNotAllowed", "At least one valid tag is required.");

    public static readonly Error MaterialMustBeDialogType =
        new("Dialog.MaterialMustBeDialogType",
            "The selected material must have material type DIALOGO_ANIMADO.");
}