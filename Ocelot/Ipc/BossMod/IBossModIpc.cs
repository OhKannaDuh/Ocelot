namespace Ocelot.Ipc.BossMod;

public interface IBossModIpc
{
    bool IsAvailable { get; }

    bool Create(string presetSerialized, bool overwrite = false);

    string? Get(string name);

    bool Delete(string name);

    bool SetActive(string name);

    bool ClearActive();

    /// <summary>VBM-only; BMR falls back to <see cref="SetActive"/>.</summary>
    bool Activate(string name);

    /// <summary>VBM-only; BMR falls back to <see cref="ClearActive"/>.</summary>
    bool Deactivate(string name);
}
