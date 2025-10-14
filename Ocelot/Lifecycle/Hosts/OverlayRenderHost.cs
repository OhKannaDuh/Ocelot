using Dalamud.Plugin;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class OverlayRenderHost(
    IEnumerable<IOnPreRender> preRender,
    IEnumerable<IOnRender> render,
    IEnumerable<IOnPostRender> postRender,
    IDalamudPluginInterface plugin,
    ILogger logger
) : BaseEventHost(logger)
{
    private readonly IOnPreRender[] preRender = preRender.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnRender[] render = render.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnPostRender[] postRender = postRender.OrderByDescending(h => h.Order).ToArray();

    public override int Count
    {
        get => preRender.Length + render.Length + postRender.Length;
    }

    public override void Start()
    {
        if (Count == 0)
        {
            return;
        }

        plugin.UiBuilder.Draw += Render;
    }

    public override void Stop()
    {
        if (Count == 0)
        {
            return;
        }

        plugin.UiBuilder.Draw -= Render;
    }

    private void Render()
    {
        SafeEach(preRender, h => h.PreRender());
        SafeEach(render, h => h.Render());
        SafeEach(postRender, h => h.PostRender());
    }
}
