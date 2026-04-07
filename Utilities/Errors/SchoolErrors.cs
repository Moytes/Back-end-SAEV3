using Utilities.Abstractions;

namespace Utilities.Errors;

public static class SchoolErrors
{
    public static readonly Error SchoolZoneNotFound =
        new("School.SchoolZoneNotFound", "School zone not found.");

    public static readonly Error SchoolNotFound =
        new("School.SchoolNotFound", "School not found.");

    public static readonly Error SchoolYearNotFound =
        new("School.SchoolYearNotFound", "School year not found.");
}