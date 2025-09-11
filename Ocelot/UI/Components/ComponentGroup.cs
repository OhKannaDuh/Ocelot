using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;

namespace Ocelot.UI.Components;

public class ComponentGroup()
{
    public readonly List<UIComponent> Components = [];

    public ComponentGroup(IEnumerable<UIComponent> components) : this()
    {
        Components.AddRange(components);
    }

    public ComponentGroup Add(string text)
    {
        return Add(new TextComponent(text));
    }

    public ComponentGroup Add(string text, Vector4 color)
    {
        return Add(new TextComponent(text, color));
    }

    public ComponentGroup Add(string text, ImFontPtr font)
    {
        return Add(new TextComponent(text, font));
    }

    public ComponentGroup Add(FontAwesomeIcon icon, Vector4 color)
    {
        return Add(new TextComponent(icon.ToString(), color, UiBuilder.IconFont));
    }

    public ComponentGroup Add(FontAwesomeIcon icon)
    {
        return Add(new TextComponent(icon.ToString(), UiBuilder.IconFont));
    }

    public ComponentGroup Add(UIComponent component)
    {
        Components.Add(component);
        return this;
    }

    public ComponentGroup AddImage(IDalamudTextureWrap texture)
    {
        return Add(new ImageComponent(texture));
    }

    public ComponentGroup AddIcon(uint iconId)
    {
        return Add(new ImageComponent(iconId));
    }

    public ComponentGroup Merge(ComponentGroup other, UIComponent connector)
    {
        if (other.Components.Count == 0)
        {
            return this;
        }

        Components.Add(connector);

        Components.AddRange(other.Components);
        return this;
    }

    public ComponentGroup Merge(ComponentGroup other)
    {
        if (other.Components.Count == 0)
        {
            return this;
        }

        Components.AddRange(other.Components);
        return this;
    }

    public ComponentGroup Prepend(ComponentGroup other, UIComponent connector)
    {
        if (other.Components.Count == 0)
        {
            return this;
        }

        Components.InsertRange(0, other.Components);
        Components.Insert(other.Components.Count, connector);

        return this;
    }

    public ComponentGroup Prepend(ComponentGroup other)
    {
        if (other.Components.Count == 0)
        {
            return this;
        }

        Components.InsertRange(0, other.Components);
        return this;
    }

    public float Width {
        get => Components.Count == 0 ? 0 : Components.Sum(c => c.GetWidth()) + ImGui.GetStyle().ItemSpacing.X * (Components.Count - 1);
    }

    public float Height {
        get => Components.Count == 0 ? 0 : Components.Sum(c => c.GetHeight());
    }

    public UiState Render()
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

        return hovered ? UiState.Hovered : UiState.None;
    }
}
