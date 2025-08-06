using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using Dalamud.Bindings.ImGui;

namespace Ocelot;

public interface UIStringComponent
{
    float GetWidth();

    bool Render();
}

public class UITextComponent(string text, Vector4 color, ImFontPtr font) : UIStringComponent
{
    public ImFontPtr Font { get; private set; } = font;

    public string Text { get; private set; } = text;

    public Vector4 Color { get; private set; } = color;

    public UITextComponent(string text)
        : this(text, Vector4.Zero, UiBuilder.DefaultFont)
    {
        Color = OcelotColor.Text;
    }

    public UITextComponent(string text, Vector4 color)
        : this(text, color, UiBuilder.DefaultFont)
    {
    }

    public UITextComponent(string text, ImFontPtr font)
        : this(text, Vector4.Zero, font)
    {
        Color = OcelotColor.Text;
    }

    public float GetWidth()
    {
        using var font = ImRaii.PushFont(Font);
        return ImGui.CalcTextSize(Text).X;
    }

    public bool Render()
    {
        using var font = ImRaii.PushFont(Font);
        ImGui.TextColored(Color, Text);
        return ImGui.IsItemHovered();
    }

    public UITextComponent WithText(string text)
    {
        Text = text;
        return this;
    }

    public UITextComponent WithColor(Vector4 color)
    {
        Color = color;
        return this;
    }

    public UITextComponent WithFont(ImFontPtr font)
    {
        Font = font;
        return this;
    }
}

public class UIImageComponent : UIStringComponent
{
    private IDalamudTextureWrap texture;

    public UIImageComponent(IDalamudTextureWrap texture)
    {
        this.texture = texture;
    }

    public UIImageComponent(uint id)
    {
        texture = Svc.Texture.GetFromGameIcon(new GameIconLookup(id)).GetWrapOrEmpty();
    }

    public float GetHeight()
    {
        return ImGui.GetFontSize();
    }


    public float GetWidth()
    {
        var aspect = (float)texture.Width / texture.Height;
        return GetHeight() * aspect;
    }

    public bool Render()
    {
        ImGui.Image(texture.Handle, new Vector2(GetWidth(), GetHeight()));
        return ImGui.IsItemHovered();
    }
}

public class UIString
{
    public readonly List<UIStringComponent> Components = [];

    public UIString()
    {
    }

    public UIString(IEnumerable<UIStringComponent> components)
    {
        Components.AddRange(components);
    }

    public static UIString Text(string text, Vector4? color = null, ImFontPtr? font = null)
    {
        var comp = new UITextComponent(text);
        if (color is { } c)
        {
            comp.WithColor(c);
        }

        if (font is { } f)
        {
            comp.WithFont(f);
        }

        return new UIString().Add(comp);
    }

    public UIString Add(string text)
    {
        return Add(new UITextComponent(text));
    }

    public UIString Add(string text, Vector4 color)
    {
        return Add(new UITextComponent(text, color));
    }

    public UIString Add(string text, ImFontPtr font)
    {
        return Add(new UITextComponent(text, font));
    }

    public UIString Add(FontAwesomeIcon icon, Vector4 color)
    {
        return Add(new UITextComponent(icon.ToString(), color, UiBuilder.IconFont));
    }

    public UIString Add(FontAwesomeIcon icon)
    {
        return Add(new UITextComponent(icon.ToString(), UiBuilder.IconFont));
    }

    public UIString Add(UIStringComponent component)
    {
        Components.Add(component);
        return this;
    }

    public UIString AddImage(IDalamudTextureWrap texture)
    {
        return Add(new UIImageComponent(texture));
    }

    public UIString AddIcon(uint iconId)
    {
        return Add(new UIImageComponent(iconId));
    }

    public float Width
    {
        get => Components.Count == 0 ? 0 : Components.Sum(c => c.GetWidth()) + ImGui.GetStyle().ItemSpacing.X * (Components.Count - 1);
    }

    public UIState Render()
    {
        var hovered = false;
        for (var i = 0; i < Components.Count; i++)
        {
            if (Components[i].Render())
            {
                hovered = true;
            }

            if (i < Components.Count - 1)
            {
                ImGui.SameLine();
            }
        }

        return hovered ? UIState.Hovered : UIState.None;
    }
}
