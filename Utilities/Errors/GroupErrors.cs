using Utilities.Abstractions;

namespace Utilities.Errors;

public static class GroupErrors
{
    public static readonly Error GroupNotFound =
        new("Group.GroupNotFound", "Group not found.");

    public static readonly Error GroupAlreadyExists =
        new("Group.GroupAlreadyExists", "This group already exists in this school for the selected school year.");

    public static readonly Error GroupHasStudents =
        new("Group.GroupHasStudents", "The group cannot be deleted because it has enrolled students.");
}