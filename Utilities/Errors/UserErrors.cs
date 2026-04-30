using Utilities.Abstractions;

namespace Utilities.Errors;

public static class UserErrors
{
    public static readonly Error EmailAlreadyExists =
        new("User.EmailAlreadyExists", "A user with that email already exists.");

    public static readonly Error UserNotFound =
        new("User.UserNotFound", "User not found.");

    public static readonly Error UserGroupAssignmentAlreadyExists =
        new("User.UserGroupAssignmentAlreadyExists", "The user is already assigned to that group for that school year.");

    public static readonly Error UserSchoolAssignmentAlreadyExists =
        new("User.UserSchoolAssignmentAlreadyExists", "The user is already assigned to that school for that school year.");

    public static readonly Error StudentAlreadyHasUserAccount =
        new("User.StudentAlreadyHasUserAccount", "The student already has a user account.");

    public static readonly Error StudentIdRequiredForStudentRole =
        new("User.StudentIdRequiredForStudentRole", "StudentId is required for the STUDENT role.");
}