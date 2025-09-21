using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Ocelot.UI.Services;

public partial class OcelotUIService
{
    public void ProgressBar(float progress, Vector2 size, string? text = null)
    {
        ImGui.ProgressBar(progress, size, text);
    }

    public void ProgressBar(float progress, string? text = null)
    {
        ImGui.ProgressBar(progress, text);
    }
}
