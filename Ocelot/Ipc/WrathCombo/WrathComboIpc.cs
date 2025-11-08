using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace Ocelot.Ipc.WrathCombo;

public class WrathComboIpc(IDalamudPluginInterface plugin) : IWrathComboIpc
{
    private readonly ICallGateSubscriber<string, string, Guid?> registerForLease =
        plugin.GetIpcSubscriber<string, string, Guid?>("WrathCombo.RegisterForLease");

    private readonly ICallGateSubscriber<Guid, object> releaseControl = plugin.GetIpcSubscriber<Guid, object>("WrathCombo.ReleaseControl");

    private readonly ICallGateSubscriber<Guid, WrathAutoRotationConfigOption, object, WrathSetResult> setAutoRotationConfigState =
        plugin.GetIpcSubscriber<Guid, WrathAutoRotationConfigOption, object, WrathSetResult>("WrathCombo.SetAutoRotationConfigState");

    private readonly ICallGateSubscriber<WrathAutoRotationConfigOption, object?> getAutoRotationConfigState =
        plugin.GetIpcSubscriber<WrathAutoRotationConfigOption, object?>("WrathCombo.GetAutoRotationConfigState");

    private readonly ICallGateSubscriber<Guid, bool, WrathSetResult> setAutoRotationState = plugin
        .GetIpcSubscriber<Guid, bool, WrathSetResult>("WrathCombo.SetAutoRotationState");

    private readonly ICallGateSubscriber<bool> getAutoRotationState = plugin
        .GetIpcSubscriber<bool>("WrathCombo.GetAutoRotationState");

    public Guid? RegisterForLease(string internalPluginName, string pluginName)
    {
        return registerForLease.InvokeFunc(internalPluginName, pluginName);
    }

    public void ReleaseControl(Guid lease)
    {
        releaseControl.InvokeAction(lease);
    }

    public WrathSetResult SetAutoRotationConfigState(Guid lease, WrathAutoRotationConfigOption option, object value)
    {
        return setAutoRotationConfigState.InvokeFunc(lease, option, value);
    }

    public object? GetAutoRotationConfigState(WrathAutoRotationConfigOption option)
    {
        return getAutoRotationConfigState.InvokeFunc(option);
    }

    public WrathSetResult SetAutoRotationState(Guid lease, bool enabled = true)
    {
        return setAutoRotationState.InvokeFunc(lease, enabled);
    }

    public bool GetAutoRotationState()
    {
        return getAutoRotationState.InvokeFunc();
    }
}
