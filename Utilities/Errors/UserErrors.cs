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

    public static readonly Error RoleNotFound =
        new("User.RoleNotFound", "The specified role was not found.");

    public static readonly Error RoleNotAllowed =
        new("User.RoleNotAllowed", "The specified role is not allowed for this operation.");
}
