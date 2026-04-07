using Utilities.Abstractions;

namespace Utilities.Errors;

public static class GroupErrors
{
    public static readonly Error GroupNotFound =
        new("Group.GroupNotFound", "Group not found.");
}