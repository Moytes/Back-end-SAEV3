using Utilities.Abstractions;

namespace Utilities.Errors;

public static class AssignmentErrors
{
    public static readonly Error AssignmentStudentNotFound =
        new("Assignment.AssignmentStudentNotFound", "Assignment student record not found.");

    public static readonly Error GroupOrStudentsRequired =
        new("Assignment.GroupOrStudentsRequired", "You must provide at least one student.");

    public static readonly Error NoStudentsResolvedForAssignment =
        new("Assignment.NoStudentsResolvedForAssignment",
            "No students were resolved for the assignment.");
}