using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.BossMod;

/// <summary>
///     BossMod / BossMod Reborn share the <c>BossMod.Presets.*</c> IPC prefix.
///     Prefer <see cref="SetActive"/> / <see cref="ClearActive"/> — those exist on both.
/// </summary>
public class BossModIpc(IDalamudPluginInterface plugin) : IBossModIpc
{
    private readonly ICallGateSubscriber<string, bool, object> create =
        plugin.GetIpcSubscriber<string, bool, object>("BossMod.Presets.Create");

    private readonly ICallGateSubscriber<string, bool> setActive =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.SetActive");

    private readonly ICallGateSubscriber<bool> clearActive =
        plugin.GetIpcSubscriber<bool>("BossMod.Presets.ClearActive");

    private readonly ICallGateSubscriber<string, bool> activate =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Activate");

    private readonly ICallGateSubscriber<string, bool> deactivate =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Deactivate");

    public void Create(string presetSerialized, bool overwrite = false)
    {
        create.InvokeAction(presetSerialized, overwrite);
    }

    public bool SetActive(string name)
    {
        return setActive.InvokeFunc(name);
    }

    public bool ClearActive()
    {
        return clearActive.InvokeFunc();
    }

    public bool Activate(string name)
    {
        try
        {
            return activate.InvokeFunc(name);
        }
        catch
        {
            // BMR does not register Activate — fall back to exclusive SetActive.
            return SetActive(name);
        }
    }

    public bool Deactivate(string name)
    {
        try
        {
            return deactivate.InvokeFunc(name);
        }
        catch
        {
            // BMR does not register Deactivate — clear whatever is active.
            return ClearActive();
        }
    }
}
