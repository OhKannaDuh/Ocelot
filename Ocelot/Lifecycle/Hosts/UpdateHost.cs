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
    private IOnPreUpdate[]? preUpdateHooks;

    private IOnUpdate[]? updateHooks;

    private IOnPostUpdate[]? postUpdateHooks;

    private IOnPreUpdate[] PreUpdateHooks =>
        preUpdateHooks ??= services.GetServices<IOnPreUpdate>().OrderByDescending(h => h.Order).ToArray();

    private IOnUpdate[] UpdateHooks =>
        updateHooks ??= services.GetServices<IOnUpdate>().OrderByDescending(h => h.Order).ToArray();

    private IOnPostUpdate[] PostUpdateHooks =>
        postUpdateHooks ??= services.GetServices<IOnPostUpdate>().OrderByDescending(h => h.Order).ToArray();

    public override int Count
    {
        get => (preUpdateHooks?.Length ?? 0) + (updateHooks?.Length ?? 0) + (postUpdateHooks?.Length ?? 0);
    }

    private bool subscribed;

    public override void Start()
    {
        var preUpdate = PreUpdateHooks;
        var update = UpdateHooks;
        var postUpdate = PostUpdateHooks;

        if (preUpdate.Length + update.Length + postUpdate.Length == 0)
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
        SafeEach(PreUpdateHooks.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "pre_update")), h => h.PreUpdate());
        SafeEach(UpdateHooks.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "update")), h => h.Update());
        SafeEach(PostUpdateHooks.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "post_update")), h => h.PostUpdate());
    }
}
