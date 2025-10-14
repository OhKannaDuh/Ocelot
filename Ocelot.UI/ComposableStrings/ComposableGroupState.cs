using System;

namespace Ocelot.UI.ComposableStrings;

[Flags]
public enum ComposableGroupState
{
    Empty         = 0,
    HoveredLeft  = 1 << 0,
    HoveredRight = 1 << 1,
    Hovered = HoveredLeft | HoveredRight
}

