using Utilities.Abstractions;

namespace Utilities.Errors;

public static class StudentErrors
{
    public static readonly Error StudentNotFound =
        new("Student.StudentNotFound", "Student not found.");

    public static readonly Error CurpAlreadyExists =
        new("Student.CurpAlreadyExists", "A student with that CURP already exists.");
}