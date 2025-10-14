using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ocelot.Graphics;

namespace Ocelot.UI.ComposableStrings;

public class Text(string text, Color color, ImFontPtr font) : IComposable
{
    private Vector2? size = null;

    public float GetWidth()
    {
        if (size == null)
        {
            using var _ = ImRaii.PushFont(font);
            size = ImGui.CalcTextSize(text);
        }

        return size.Value.X;
    }

    public float GetHeight()
    {
        if (size == null)
        {
            using var _ = ImRaii.PushFont(font);
            size = ImGui.CalcTextSize(text);
        }

        return size.Value.Y;
    }

    public bool Render()
    {
        using var _ = ImRaii.PushFont(font);
        ImGui.TextColored(color.ToRgba(), text);
        return ImGui.IsItemHovered();
    }
}
