using System.Numerics;
using Dalamud.Interface;

namespace Ocelot.UI.Components;

public class FontAwesomeIconComponent : TextComponent
{
    public FontAwesomeIconComponent(FontAwesomeIcon icon)
        : base(icon.ToIconString(), UiBuilder.IconFont) { }

    public FontAwesomeIconComponent(FontAwesomeIcon icon, Vector4 color)
        : base(icon.ToIconString(), color, UiBuilder.IconFont) { }
}
