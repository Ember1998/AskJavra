using System.ComponentModel;

namespace AskJavra.Enums
{
    public enum FeedStatus
    {
        [Description("Open")]
        Open = 101,
        [Description("Closed")]
        Closed = 102,
        [Description("Resolved")]
        Resolved = 103
    }
}
