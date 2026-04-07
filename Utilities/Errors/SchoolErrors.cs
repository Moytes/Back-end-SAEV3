using Utilities.Abstractions;

namespace Utilities.Errors;

public class SchoolErrors
{
    public static Error SchoolZoneNotFound => new Error("SchoolZoneNotFound", "The specified school zone does not exist.");
}
