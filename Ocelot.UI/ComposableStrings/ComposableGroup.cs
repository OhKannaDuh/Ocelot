using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using Ocelot.UI.Services;

namespace Ocelot.UI.ComposableStrings;

public partial class ComposableGroup(IBrandingService branding, ITextureProvider textures)
{
    private readonly List<IComposable> composables = [];
    
    public float Width
    {
        get => composables.Count == 0 ? 0 : composables.Sum(c => c.GetWidth()) + ImGui.GetStyle().ItemSpacing.X * (composables.Count - 1);
    }

    public float Height
    {
        get => composables.Count == 0 ? 0 : composables.Sum(c => c.GetHeight());
    }

    public bool Render()
    {
        var anyHovered = false;
        for (var i = 0; i < composables.Count; i++)
        {
            if (composables[i].Render())
            {
                anyHovered = true;
            }

            if (i < composables.Count - 1)
            {
                ImGui.SameLine();
            }
        }

        return anyHovered;
    }
}
