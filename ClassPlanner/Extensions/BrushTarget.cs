using System;

namespace ClassPlanner.Extensions;

[Flags]
public enum BrushTarget
{
    None = 0,
    Background = 1,
    Foreground = 2,
    BorderBrush = 4
}
