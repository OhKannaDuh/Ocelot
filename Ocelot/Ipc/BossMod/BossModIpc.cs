using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.BossMod;

public class BossModIpc(IDalamudPluginInterface plugin) : IBossModIpc
{
    private readonly ICallGateSubscriber<string, bool, object> create = plugin.GetIpcSubscriber<string, bool, object>("BossMod.Presets.Create");

    private readonly ICallGateSubscriber<string, bool> activate = plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Activate");

    private readonly ICallGateSubscriber<string, bool> deactivate = plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Deactivate");

    public void Create(string presetSerialized, bool overwrite = false)
    {
        create.InvokeAction(presetSerialized, overwrite);
    }

    public bool Activate(string name)
    {
        return activate.InvokeFunc(name);
    }

    public bool Deactivate(string name)
    {
        return deactivate.InvokeFunc(name);
    }
}
