using Dalamud.Plugin;
using ECommons;
using Ocelot.ECommons.Services;
using Ocelot.Lifecycle;

namespace Ocelot.ECommons;

internal sealed class ECommons(IDalamudPluginInterface pluginInterface, OcelotPlugin plugin, IECommonsInitProvider init) : IOnLoad, IOnStop
{
    public void OnLoad()
    {
        ECommonsMain.Init(pluginInterface, plugin, init.GetModules());
    }

    public void OnStop()
    {
        ECommonsMain.Dispose();
    }
}
