using Utilities.Abstractions;

namespace Utilities.Errors;

public static class GroupErrors
{
    public static readonly Error GroupNotFound =
        new("Group.GroupNotFound", "Group not found.");

    public static readonly Error GroupAlreadyExists =
        new("Group.GroupAlreadyExists", "This group already exists in this school for the selected school year.");
}