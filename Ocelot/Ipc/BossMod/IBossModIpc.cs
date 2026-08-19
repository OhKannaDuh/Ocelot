namespace Ocelot.Ipc.BossMod;

public interface IBossModIpc
{
    bool IsAvailable { get; }

    bool Create(string presetSerialized, bool overwrite = false);

    string? Get(string name);

    bool Delete(string name);

    bool SetActive(string name);

    bool ClearActive();

    string? GetActive();

    /// <summary>VBM Activate, or SetActive on BMR.</summary>
    bool Activate(string name);

    /// <summary>VBM Deactivate, or ClearActive on BMR only when <paramref name="name"/> is active.</summary>
    bool Deactivate(string name);

    /// <summary>Overlay a track on an existing preset without rewriting it. No-op if the IPC is missing.</summary>
    bool AddTransientStrategy(string presetName, string moduleTypeName, string trackName, string value);

    bool ClearTransientPresetStrategies(string presetName);
}
