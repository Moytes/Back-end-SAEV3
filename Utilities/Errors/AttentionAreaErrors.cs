using Utilities.Abstractions;

namespace Utilities.Errors;

public static class AttentionAreaErrors
{
    public static readonly Error AttentionAreaNotFound =
        new("AttentionArea.AttentionAreaNotFound", "Attention area not found.");

    public static readonly Error DuplicateAttentionAreasInRequest =
        new("AttentionArea.DuplicateAttentionAreasInRequest",
            "The request contains duplicated attention areas.");
}