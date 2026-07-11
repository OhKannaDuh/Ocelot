using Dalamud.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class OverlayRenderHost(
    IServiceProvider services,
    IDalamudPluginInterface plugin,
    ILogger<OverlayRenderHost> logger
) : BaseEventHost(logger)
{
    private IOnPreRender[]? preRenderHooks;

    private IOnRender[]? renderHooks;

    private IOnPostRender[]? postRenderHooks;

    private IOnPreRender[] PreRenderHooks =>
        preRenderHooks ??= services.GetServices<IOnPreRender>().OrderByDescending(h => h.Order).ToArray();

    private IOnRender[] RenderHooks =>
        renderHooks ??= services.GetServices<IOnRender>().OrderByDescending(h => h.Order).ToArray();

    private IOnPostRender[] PostRenderHooks =>
        postRenderHooks ??= services.GetServices<IOnPostRender>().OrderByDescending(h => h.Order).ToArray();

    public override int Count
    {
        get => (preRenderHooks?.Length ?? 0) + (renderHooks?.Length ?? 0) + (postRenderHooks?.Length ?? 0);
    }

    private bool subscribed;

    public override void Start()
    {
        var preRender = PreRenderHooks;
        var render = RenderHooks;
        var postRender = PostRenderHooks;

        if (preRender.Length + render.Length + postRender.Length == 0)
        {
            return;
        }

        plugin.UiBuilder.Draw += OnDraw;
        subscribed = true;
    }

    public override void Stop()
    {
        if (!subscribed)
        {
            return;
        }

        plugin.UiBuilder.Draw -= OnDraw;
        subscribed = false;
    }

    private void OnDraw()
    {
        SafeEach(PreRenderHooks, h => h.PreRender());
        SafeEach(RenderHooks, h => h.Render());
        SafeEach(PostRenderHooks, h => h.PostRender());
    }
}
