using System.ComponentModel;

namespace AskJavra.Enums
{
    public enum PostType
    {
        [Description("Personal Memo")]
        memo,
        [Description("Org-Wide")]
        org_wide,
        [Description("Public")]
        _public
    }
}
