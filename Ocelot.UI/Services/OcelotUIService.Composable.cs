using Dalamud.Bindings.ImGui;
using Ocelot.UI.ComposableStrings;

namespace Ocelot.UI.Services;

public partial class OcelotUIService
{
    public ComposableGroup Compose()
    {
        return new ComposableGroup(branding, textures);
    }

    public ComposableGroupState Render(ComposableGroup left, ComposableGroup right)
    {
        var state = ComposableGroupState.Empty;

        if (left.Render())
        {
            state |= ComposableGroupState.HoveredLeft;
        }

        ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - right.Width);

        if (right.Render())
        {
            state |= ComposableGroupState.HoveredRight;
        }

        return state;
    }
}
