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

    bool IsCurrentJobAutoRotationReady();

    /// <summary>
    ///     Enables Simple/Advanced combos for the current job (or locks existing setup) for Auto-Rotation.
    /// </summary>
    WrathSetResult SetCurrentJobAutoRotationReady(Guid lease);

    Dictionary<WrathComboStateKeys, bool>? GetComboState(string comboInternalName);

    WrathSetResult SetComboState(Guid lease, string comboInternalName, bool comboState = true, bool autoState = true);

    bool GetComboOptionState(string optionName);

    WrathSetResult SetComboOptionState(Guid lease, string optionName, bool state = true);

    /// <summary>
    ///     Occult Crescent Phantom Job parent preset name (e.g. Chemist 10 → Phantom_Chemist).
    ///     <paramref name="phantomJobId"/> is 0–23, not a ClassJob ID.
    /// </summary>
    string? GetOccultParentComboName(uint phantomJobId);

    /// <summary>
    ///     Child option internal names for a Phantom Job pack (excludes Phantom_RestrictToBuff).
    /// </summary>
    List<string>? GetOccultOptionNames(uint phantomJobId);

    /// <summary>
    ///     Enables or disables the Phantom Job parent + all options under the lease.
    ///     Does not enable Phantom_RestrictToBuff or change config sliders.
    /// </summary>
    WrathSetResult SetOccultReadyForPhantomJob(Guid lease, uint phantomJobId, bool enabled = true);
}
