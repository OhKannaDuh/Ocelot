using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Ocelot.Services.Logger;

namespace Ocelot.Lifecycle.Hosts;

public class UpdateHost(
    IEnumerable<IOnPreUpdate> preUpdate,
    IEnumerable<IOnUpdate> update,
    IEnumerable<IOnPostUpdate> postUpdate,
    IFramework framework,
    ILoggerService logger
) : BaseEventHost(logger)
{
    private readonly IOnPreUpdate[] preUpdate = preUpdate.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnUpdate[] update = update.OrderByDescending(h => h.Order).ToArray();

    private readonly IOnPostUpdate[] postUpdate = postUpdate.OrderByDescending(h => h.Order).ToArray();

    public override int Count {
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

    public void Update(IFramework _)
    {
        SafeEach(preUpdate, h => h.PreUpdate());
        SafeEach(update, h => h.Update());
        SafeEach(postUpdate, h => h.PostUpdate());
    }
}
