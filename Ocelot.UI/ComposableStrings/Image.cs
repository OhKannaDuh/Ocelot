using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;

namespace Ocelot.UI.ComposableStrings;

public class Image(IDalamudTextureWrap texture) : IComposable
{
    private Vector4 tint = Vector4.One;

    private Vector2 size = Vector2.Zero;

    private float scale = 1.0f;

    private string? Tooltip;

    public Image(ITextureProvider textures, uint id) : this(textures.GetFromGameIcon(new GameIconLookup(id)).GetWrapOrEmpty())
    {
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

    public float GetHeight()
    {
        if (size.Y != 0)
        {
            return size.Y * scale;
        }

        return ImGui.GetFontSize() * scale;
    }

    public bool Render()
    {
        ImGui.Image(texture.Handle, new Vector2(GetWidth(), GetHeight()), tint);
        return ImGui.IsItemHovered();
        // if (hovered && Tooltip != null)
        // {
        //     ImGui.SetTooltip(Tooltip);
        //     return false;
        // }

        // return hovered;
    }

    public Image WithTint(Vector4 tint)
    {
        this.tint = tint;
        return this;
    }

    public Image WithAlpha(float alpha)
    {
        return WithTint(new Vector4(1, 1, 1, alpha));
    }

    public Image WithSize(Vector2 size)
    {
        this.size = size;
        return this;
    }

    public Image WithWidth(float width)
    {
        return WithSize(size with { X = width });
    }

    public Image WithHeight(float height)
    {
        return WithSize(size with { Y = height });
    }

    public Image WithScale(float scale)
    {
        this.scale = scale;
        return this;
    }

    public Image WithTooltip(string? tooltip)
    {
        Tooltip = tooltip;
        return this;
    }
}
