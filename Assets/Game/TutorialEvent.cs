// Ported from TutorialEvent.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class TutorialEvent : Event
{
    public const string TRIGGER_NEXT = "trigger_next";
    public string actionName;
    public Dictionary<string, object> infos;

    // AS3: super(arg1, arg3, arg4) -> Starling Event(type, bubbles, data); arg4 ends up as the event's data.
    public TutorialEvent(string arg1, string arg2, bool arg3 = true, bool arg4 = false, Dictionary<string, object> arg5 = null)
        : base(arg1, arg3, arg4)
    {
        this.actionName = arg2;
        this.infos = arg5;
    }
}
