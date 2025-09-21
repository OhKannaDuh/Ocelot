using Ocelot.Graphics;

namespace Ocelot.UI.Services;

public partial class OcelotUIService(IBrandingService branding) : IUIService
{
    private uint ColorToImGui(Color? color, Color? fallback = null)
    {
        return (color ?? fallback ?? branding.Text).ToRgba();
    }
}
