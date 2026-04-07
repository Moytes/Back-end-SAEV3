using Utilities.Abstractions;

namespace Utilities.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists => new Error("EmailAlreadyExists", "The provided email is already in use.");
}
