using Dalamud.Plugin.Services;
using Ocelot.Graphics;

namespace Ocelot.UI.Services;

public partial class OcelotUIService(IBrandingService branding, ITextureProvider textures) : IUIService
{
    private uint ColorToImGui(Color? color, Color? fallback = null)
    {
        return (color ?? fallback ?? branding.Text).ToRgba();
    }
}
