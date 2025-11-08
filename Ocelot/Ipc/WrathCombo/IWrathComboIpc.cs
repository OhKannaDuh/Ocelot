namespace Ocelot.Ipc.WrathCombo;

public interface IWrathComboIpc
{
    Guid? RegisterForLease(string internalPluginName, string pluginName);

    void ReleaseControl(Guid lease);

    // Value varies based on the option
    WrathSetResult SetAutoRotationConfigState(Guid lease, WrathAutoRotationConfigOption option, object value);

    object? GetAutoRotationConfigState(WrathAutoRotationConfigOption option);

    WrathSetResult SetAutoRotationState(Guid lease, bool enabled = true);

    bool GetAutoRotationState();
}
