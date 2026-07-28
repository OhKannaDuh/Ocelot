using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Services.Gate;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class UpdateHost(
    IServiceProvider services,
    IFramework framework,
    IGateService gate,
    ILogger<UpdateHost> logger
) : BaseEventHost(logger)
{
    private readonly Lazy<IOnPreUpdate[]> preUpdateHooks = new(() =>
        services.GetServices<IOnPreUpdate>().OrderByDescending(h => h.Order).ToArray());

    private readonly Lazy<IOnUpdate[]> updateHooks = new(() =>
        services.GetServices<IOnUpdate>().OrderByDescending(h => h.Order).ToArray());

    private readonly Lazy<IOnPostUpdate[]> postUpdateHooks = new(() =>
        services.GetServices<IOnPostUpdate>().OrderByDescending(h => h.Order).ToArray());

    public override int Count
    {
        get
        {
            var count = 0;
            if (preUpdateHooks.IsValueCreated)
            {
                count += preUpdateHooks.Value.Length;
            }

            if (updateHooks.IsValueCreated)
            {
                count += updateHooks.Value.Length;
            }

            if (postUpdateHooks.IsValueCreated)
            {
                count += postUpdateHooks.Value.Length;
            }

            return count;
        }
    }

    private bool subscribed;

    public override void Start()
    {
        if (preUpdateHooks.Value.Length + updateHooks.Value.Length + postUpdateHooks.Value.Length == 0)
        {
            return;
        }

        framework.Update += OnFrameworkUpdate;
        subscribed = true;
    }

    public override void Stop()
    {
        if (!subscribed)
        {
            return;
        }

        framework.Update -= OnFrameworkUpdate;
        subscribed = false;
    }

    private void OnFrameworkUpdate(IFramework _)
    {
        SafeEach(preUpdateHooks.Value.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "pre_update")), h => h.PreUpdate());
        SafeEach(updateHooks.Value.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "update")), h => h.Update());
        SafeEach(postUpdateHooks.Value.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "post_update")), h => h.PostUpdate());
    }
}
