using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices;

namespace Ocelot.UI.Components;

public class ImageComponent(IDalamudTextureWrap texture) : UIComponent
{
    public Vector4 tint = Vector4.One;

    public Vector2 size = Vector2.Zero;

    public float scale = 1.0f;

    public string? Tooltip { get; private set; }

    public ImageComponent(uint id) : this(Svc.Texture.GetFromGameIcon(new GameIconLookup(id)).GetWrapOrEmpty()) { }

    public float GetHeight()
    {
        if (size.Y != 0)
        {
            return size.Y * scale;
        }

        return ImGui.GetFontSize() * scale;
    }

    public float GetWidth()
    {
        if (size.X != 0)
        {
            return size.X * scale;
        }

        var aspect = (float)texture.Width / texture.Height;
        return GetHeight() * aspect;
    }

    public bool Render()
    {
        ImGui.Image(texture.Handle, new Vector2(GetWidth(), GetHeight()), tint);
        var hovered = ImGui.IsItemHovered();
        if (hovered && Tooltip != null)
        {
            ImGui.SetTooltip(Tooltip);
            return false;
        }

        return hovered;
    }

    public ImageComponent WithTint(Vector4 tint)
    {
        this.tint = tint;
        return this;
    }

    public ImageComponent WithAlpha(float alpha)
    {
        return WithTint(new Vector4(1, 1, 1, alpha));
    }

    public ImageComponent WithSize(Vector2 size)
    {
        this.size = size;
        return this;
    }

    public ImageComponent WithWidth(float width)
    {
        return WithSize(size with { X = width });
    }

    public ImageComponent WithHeight(float height)
    {
        return WithSize(size with { Y = height });
    }

    public ImageComponent WithScale(float scale)
    {
        this.scale = scale;
        return this;
    }

    public ImageComponent WithTooltip(string? tooltip)
    {
        Tooltip = tooltip;
        return this;
    }
}
