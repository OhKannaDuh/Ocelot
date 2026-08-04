using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.BossMod;

/// <summary>BossMod / BossMod Reborn shared <c>BossMod.Presets.*</c> IPC.</summary>
public class BossModIpc(IDalamudPluginInterface plugin) : IBossModIpc
{
    private readonly ICallGateSubscriber<string, bool, bool> create =
        plugin.GetIpcSubscriber<string, bool, bool>("BossMod.Presets.Create");

    private readonly ICallGateSubscriber<string, string?> get =
        plugin.GetIpcSubscriber<string, string?>("BossMod.Presets.Get");

    private readonly ICallGateSubscriber<string, bool> delete =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Delete");

    private readonly ICallGateSubscriber<string, bool> setActive =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.SetActive");

    private readonly ICallGateSubscriber<bool> clearActive =
        plugin.GetIpcSubscriber<bool>("BossMod.Presets.ClearActive");

    private readonly ICallGateSubscriber<string?> getActive =
        plugin.GetIpcSubscriber<string?>("BossMod.Presets.GetActive");

    private readonly ICallGateSubscriber<string, bool> activate =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Activate");

    private readonly ICallGateSubscriber<string, bool> deactivate =
        plugin.GetIpcSubscriber<string, bool>("BossMod.Presets.Deactivate");

    public bool IsAvailable
    {
        get
        {
            try
            {
                return create.HasFunction && setActive.HasFunction && clearActive.HasFunction;
            }
            catch
            {
                return false;
            }
        }
    }

    public bool Create(string presetSerialized, bool overwrite = false)
    {
        try
        {
            return create.HasFunction && create.InvokeFunc(presetSerialized, overwrite);
        }
        catch
        {
            return false;
        }
    }

    public string? Get(string name)
    {
        try
        {
            return get.HasFunction ? get.InvokeFunc(name) : null;
        }
        catch
        {
            return null;
        }
    }

    public bool Delete(string name)
    {
        try
        {
            return delete.HasFunction && delete.InvokeFunc(name);
        }
        catch
        {
            return false;
        }
    }

    public bool SetActive(string name)
    {
        try
        {
            return setActive.HasFunction && setActive.InvokeFunc(name);
        }
        catch
        {
            return false;
        }
    }

    public bool ClearActive()
    {
        try
        {
            return clearActive.HasFunction && clearActive.InvokeFunc();
        }
        catch
        {
            return false;
        }
    }

    public string? GetActive()
    {
        try
        {
            return getActive.HasFunction ? getActive.InvokeFunc() : null;
        }
        catch
        {
            return null;
        }
    }

    public bool Activate(string name)
    {
        try
        {
            if (activate.HasFunction)
            {
                return activate.InvokeFunc(name);
            }
        }
        catch
        {
            // BMR: Activate not registered.
        }

        return SetActive(name);
    }

    public bool Deactivate(string name)
    {
        try
        {
            if (deactivate.HasFunction)
            {
                return deactivate.InvokeFunc(name);
            }
        }
        catch
        {
            // BMR: Deactivate not registered.
        }

        // BMR only has a single active preset slot — clear only if BOCCHI AI is the one active.
        // Blind ClearActive() was wiping whatever the user had selected after travel.
        string? active = GetActive();
        if (active == null)
        {
            return true;
        }

        if (!string.Equals(active, name, StringComparison.Ordinal))
        {
            return true;
        }

        return ClearActive();
    }
}
