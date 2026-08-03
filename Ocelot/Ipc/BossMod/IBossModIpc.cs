namespace Ocelot.Ipc.BossMod;

public interface IBossModIpc
{
    void Create(string presetSerialized, bool overwrite = false);

    /// <summary>Clear active presets and activate <paramref name="name"/> (VBM + BMR).</summary>
    bool SetActive(string name);

    /// <summary>Clear all active presets (VBM + BMR).</summary>
    bool ClearActive();

    /// <summary>Add a preset to the active set. VBM only — BMR does not expose this.</summary>
    bool Activate(string name);

    /// <summary>Remove a preset from the active set. VBM only — BMR does not expose this.</summary>
    bool Deactivate(string name);
}
