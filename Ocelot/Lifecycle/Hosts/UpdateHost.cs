using Dalamud.Plugin.Services;
using Ocelot.Services.Gate;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class UpdateHost(
    IEnumerable<IOnPreUpdate> preUpdate,
    IEnumerable<IOnUpdate> update,
    IEnumerable<IOnPostUpdate> postUpdate,
    IFramework framework,
    IGateService gate,
    ILogger logger
) : BaseEventHost(logger)
{
    private readonly IOnPreUpdate[] preUpdate = preUpdate.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnUpdate[] update = update.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnPostUpdate[] postUpdate = postUpdate.OrderByDescending(h => h.Order).ToArray();
    
    public override int Count
    {
        get => preUpdate.Length + update.Length + postUpdate.Length;
    }

    public override void Start()
    {
        if (Count == 0)
        {
            return;
        }

        framework.Update += Update;
    }

    public override void Stop()
    {
        if (Count == 0)
        {
            return;
        }

        framework.Update -= Update;
    }

    private void Update(IFramework _)
    {
        SafeEach(preUpdate.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "pre_update")), h => h.PreUpdate());
        SafeEach(update.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "update")), h => h.Update());
        SafeEach(postUpdate.Where(h => h.UpdateLimit.ShouldUpdate(gate, h, "post_update")), h => h.PostUpdate());
    }
}
