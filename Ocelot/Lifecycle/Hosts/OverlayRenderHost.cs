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
    private readonly Lazy<IOnPreRender[]> preRenderHooks = new(() =>
        services.GetServices<IOnPreRender>().OrderByDescending(h => h.Order).ToArray());

    private readonly Lazy<IOnRender[]> renderHooks = new(() =>
        services.GetServices<IOnRender>().OrderByDescending(h => h.Order).ToArray());

    private readonly Lazy<IOnPostRender[]> postRenderHooks = new(() =>
        services.GetServices<IOnPostRender>().OrderByDescending(h => h.Order).ToArray());

    public override int Count
    {
        get
        {
            var count = 0;
            if (preRenderHooks.IsValueCreated)
            {
                count += preRenderHooks.Value.Length;
            }

            if (renderHooks.IsValueCreated)
            {
                count += renderHooks.Value.Length;
            }

            if (postRenderHooks.IsValueCreated)
            {
                count += postRenderHooks.Value.Length;
            }

            return count;
        }
    }

    private bool subscribed;

    public override void Start()
    {
        if (preRenderHooks.Value.Length + renderHooks.Value.Length + postRenderHooks.Value.Length == 0)
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
        SafeEach(preRenderHooks.Value, h => h.PreRender());
        SafeEach(renderHooks.Value, h => h.Render());
        SafeEach(postRenderHooks.Value, h => h.PostRender());
    }
}
