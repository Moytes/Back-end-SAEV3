using Utilities.Abstractions;

namespace Utilities.Errors;

public static class StudentErrors
{
    public static readonly Error StudentNotFound =
        new("Student.StudentNotFound", "Student not found.");

    public static readonly Error CurpAlreadyExists =
        new("Student.CurpAlreadyExists", "A student with that CURP already exists.");

    public static readonly Error AccountCredentialsRequired =
        new("Student.AccountCredentialsRequired", "Account email and password are required for the selected education level.");

    public static readonly Error TutorNotFound =
        new("Student.TutorNotFound", "Tutor not found for this student.");
}
