using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;

namespace Ocelot.UI.Components;

public static class Component
{
    public static TextComponent Text(string text, Vector4? color = null, ImFontPtr? font = null)
    {
        var comp = new TextComponent(text);
        if (color is { } c)
        {
            comp.WithColor(c);
        }

        if (font is { } f)
        {
            comp.WithFont(f);
        }

        return comp;
    }

    public static TextComponent Text(string text, ImFontPtr font)
    {
        return Text(text, null, font);
    }

    public static ImageComponent Image(IDalamudTextureWrap texture)
    {
        return new ImageComponent(texture);
    }

    public static ImageComponent Image(uint iconId)
    {
        return new ImageComponent(iconId);
    }

    public static ImageComponent Icon(uint iconId)
    {
        return new ImageComponent(iconId);
    }

    public static FontAwesomeIconComponent Icon(FontAwesomeIcon icon)
    {
        return new FontAwesomeIconComponent(icon);
    }

    public static FontAwesomeIconComponent Icon(FontAwesomeIcon icon, Vector4 color)
    {
        return new FontAwesomeIconComponent(icon, color);
    }

    public static ButtonComponent Button(string id)
    {
        return new ButtonComponent(id);
    }

    public static ButtonComponent Button(IEnumerable<UIComponent> components, string id)
    {
        return new ButtonComponent(components, id);
    }

    public static ButtonComponent Button(ComponentGroup contents, string id)
    {
        return new ButtonComponent(contents, id);
    }

    public static ButtonComponent IconButton(uint icon, string id)
    {
        return new ButtonComponent([
            Icon(icon),
        ], id).WithPadding(new Vector2(3, 0));
    }

    public static ButtonComponent IconButton(FontAwesomeIcon icon, string id)
    {
        return new ButtonComponent([
            Icon(icon),
        ], id).WithPadding(new Vector2(3, 0));
    }
}
