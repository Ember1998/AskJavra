﻿using System.ComponentModel;

namespace AskJavra.Enums
{
    public enum FeedStatus
    {
        [Description("Open")]
        open = 100,
        [Description("Rsolved")]
        resolved = 200,
        [Description("unanswered")]
        unanswered = 300,
        [Description("Closed")]
        closed = 400
    }
}
