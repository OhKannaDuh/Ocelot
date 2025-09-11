using System.Collections.Generic;
using System.Linq;
using Ocelot.Intents;
using Ocelot.Modules;
using Ocelot.Services;

namespace Ocelot;

public sealed class UpdateScheduler
{
    private IReadOnlyList<IUpdatable> updateable = [];

    internal void Refresh()
    {
        updateable = OcelotServices.Container.GetAll<IUpdatable>().ToList();
    }

    public void Update(UpdateContext context)
    {
        var candidates = updateable.Where(u => {
            if (u is IToggleable { IsEnabled: false })
            {
                return false;
            }

            return u.UpdateLimit.ShouldUpdate(u, context);
        }).ToList();

        foreach (var c in candidates)
        {
            c.PreUpdate(context);
        }

        foreach (var c in candidates)
        {
            c.Update(context);
        }

        foreach (var c in candidates)
        {
            c.PostUpdate(context);
        }
    }
}
