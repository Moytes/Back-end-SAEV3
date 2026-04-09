using Utilities.Abstractions;

namespace Utilities.Errors;

public static class TEAErrors
{
    public static readonly Error ScreeningNotFound =
        new("TEA.ScreeningNotFound", "TEA screening not found.");

    public static readonly Error IndicatorNotFound =
        new("TEA.IndicatorNotFound", "One or more TEA indicators were not found.");

    public static readonly Error DuplicateIndicatorsInRequest =
        new("TEA.DuplicateIndicatorsInRequest", "The request contains duplicated indicators.");

    public static readonly Error InvalidFrequency =
        new("TEA.InvalidFrequency", "Frequency must be between 0 and 3.");

    public static readonly Error InvalidIntensity =
        new("TEA.InvalidIntensity", "Intensity must be between 0 and 3.");
}