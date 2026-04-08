using Utilities.Abstractions;

namespace Utilities.Errors;

public static class RegistrationErrors
{
    public static readonly Error StudentAlreadyRegisteredInSchoolYear =
        new("Registration.StudentAlreadyRegisteredInSchoolYear",
            "The student is already registered in that school year.");
}