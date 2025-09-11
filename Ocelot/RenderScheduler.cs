using System.Collections.Generic;
using System.Linq;
using Ocelot.Intents;
using Ocelot.Services;
using Ocelot.Windows;

namespace Ocelot;

public sealed class RenderScheduler
{
    private IReadOnlyList<IRenderable> renderables = [];

    internal void Refresh()
    {
        renderables = OcelotServices.Container.GetAll<IRenderable>().ToList();
    }

    public void Render(RenderContext context)
    {
        foreach (var r in renderables)
        {
            if (r is IToggleable { IsEnabled: false })
            {
                continue;
            }

            r.Render(context);
        }
    }
}
