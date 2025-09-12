using Dalamud.Plugin;
using ECommons;
using ECommons.DalamudServices;
using Ocelot.Lifecycle;

namespace Ocelot.ECommons;

internal sealed class ECommons(IDalamudPluginInterface pluginInterface, IDalamudPlugin plugin) : IOnStart, IOnStop
{
    public void OnStart()
    {
        ECommonsMain.Init(pluginInterface, plugin);
        Svc.Log.Info("ECommons started");
    }

    public void OnStop()
    {
        Svc.Log.Info("ECommons stopped");
        ECommonsMain.Dispose();
    }
}
