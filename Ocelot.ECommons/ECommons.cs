using Dalamud.Plugin;
using ECommons;
using Ocelot.Lifecycle;

namespace Ocelot.ECommons;

internal sealed class ECommons(IDalamudPluginInterface pluginInterface, IDalamudPlugin plugin) : IOnStart, IOnStop
{
    public void OnStart()
    {
        ECommonsMain.Init(pluginInterface, plugin, Module.DalamudReflector, Module.ObjectFunctions);
    }

    public void OnStop()
    {
        ECommonsMain.Dispose();
    }
}
