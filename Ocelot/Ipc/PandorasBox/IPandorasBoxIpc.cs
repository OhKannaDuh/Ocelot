namespace Ocelot.Ipc.PandorasBox;

/// <summary>Optional Pandora's Box IPC (feature toggles).</summary>
public interface IPandorasBoxIpc
{
    bool IsAvailable { get; }

    /// <summary>Null when Pandora is missing or the feature name is unknown.</summary>
    bool? GetFeatureEnabledInternal(string internalName);

    void SetFeatureEnabledInternal(string internalName, bool enabled);
}
