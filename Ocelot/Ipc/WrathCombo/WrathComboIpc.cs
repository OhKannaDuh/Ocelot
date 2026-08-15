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

    private readonly ICallGateSubscriber<bool> isCurrentJobAutoRotationReady = plugin
        .GetIpcSubscriber<bool>("WrathCombo.IsCurrentJobAutoRotationReady");

    private readonly ICallGateSubscriber<Guid, WrathSetResult> setCurrentJobAutoRotationReady = plugin
        .GetIpcSubscriber<Guid, WrathSetResult>("WrathCombo.SetCurrentJobAutoRotationReady");

    private readonly ICallGateSubscriber<string, Dictionary<WrathComboStateKeys, bool>?> getComboState = plugin
        .GetIpcSubscriber<string, Dictionary<WrathComboStateKeys, bool>?>("WrathCombo.GetComboState");

    private readonly ICallGateSubscriber<Guid, string, bool, bool, WrathSetResult> setComboState = plugin
        .GetIpcSubscriber<Guid, string, bool, bool, WrathSetResult>("WrathCombo.SetComboState");

    private readonly ICallGateSubscriber<string, bool> getComboOptionState = plugin
        .GetIpcSubscriber<string, bool>("WrathCombo.GetComboOptionState");

    private readonly ICallGateSubscriber<Guid, string, bool, WrathSetResult> setComboOptionState = plugin
        .GetIpcSubscriber<Guid, string, bool, WrathSetResult>("WrathCombo.SetComboOptionState");

    private readonly ICallGateSubscriber<uint, string?> getOccultParentComboName = plugin
        .GetIpcSubscriber<uint, string?>("WrathCombo.GetOccultParentComboName");

    private readonly ICallGateSubscriber<uint, List<string>?> getOccultOptionNames = plugin
        .GetIpcSubscriber<uint, List<string>?>("WrathCombo.GetOccultOptionNames");

    private readonly ICallGateSubscriber<Guid, uint, bool, WrathSetResult> setOccultReadyForPhantomJob = plugin
        .GetIpcSubscriber<Guid, uint, bool, WrathSetResult>("WrathCombo.SetOccultReadyForPhantomJob");

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

    public bool IsCurrentJobAutoRotationReady()
    {
        return isCurrentJobAutoRotationReady.InvokeFunc();
    }

    public WrathSetResult SetCurrentJobAutoRotationReady(Guid lease)
    {
        return setCurrentJobAutoRotationReady.InvokeFunc(lease);
    }

    public Dictionary<WrathComboStateKeys, bool>? GetComboState(string comboInternalName)
    {
        return getComboState.InvokeFunc(comboInternalName);
    }

    public WrathSetResult SetComboState(Guid lease, string comboInternalName, bool comboState = true, bool autoState = true)
    {
        return setComboState.InvokeFunc(lease, comboInternalName, comboState, autoState);
    }

    public bool GetComboOptionState(string optionName)
    {
        return getComboOptionState.InvokeFunc(optionName);
    }

    public WrathSetResult SetComboOptionState(Guid lease, string optionName, bool state = true)
    {
        return setComboOptionState.InvokeFunc(lease, optionName, state);
    }

    public string? GetOccultParentComboName(uint phantomJobId)
    {
        return getOccultParentComboName.InvokeFunc(phantomJobId);
    }

    public List<string>? GetOccultOptionNames(uint phantomJobId)
    {
        return getOccultOptionNames.InvokeFunc(phantomJobId);
    }

    public WrathSetResult SetOccultReadyForPhantomJob(Guid lease, uint phantomJobId, bool enabled = true)
    {
        return setOccultReadyForPhantomJob.InvokeFunc(lease, phantomJobId, enabled);
    }
}
