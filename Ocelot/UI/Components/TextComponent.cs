using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;

namespace Ocelot.UI.Components;

public class TextComponent(string text, Vector4 color, ImFontPtr font) : UIComponent
{
    public ImFontPtr Font { get; private set; } = font;

    public string Text { get; private set; } = text;

    public Vector4 Color { get; private set; } = color;

    public TextComponent(string text)
        : this(text, Vector4.Zero, UiBuilder.DefaultFont)
    {
        Color = OcelotColor.Text;
    }

    public TextComponent(string text, Vector4 color)
        : this(text, color, UiBuilder.DefaultFont) { }

    public TextComponent(string text, ImFontPtr font)
        : this(text, Vector4.Zero, font)
    {
        Color = OcelotColor.Text;
    }

    public float GetWidth()
    {
        using var font = ImRaii.PushFont(Font);
        return ImGui.CalcTextSize(Text).X;
    }

    public float GetHeight()
    {
        using var font = ImRaii.PushFont(Font);
        return ImGui.CalcTextSize(Text).Y;
    }

    public bool Render()
    {
        using var font = ImRaii.PushFont(Font);
        ImGui.TextColored(Color, Text);
        return ImGui.IsItemHovered();
    }

    public TextComponent WithText(string text)
    {
        Text = text;
        return this;
    }

    public TextComponent WithColor(Vector4 color)
    {
        Color = color;
        return this;
    }

    public TextComponent WithFont(ImFontPtr font)
    {
        Font = font;
        return this;
    }
}
