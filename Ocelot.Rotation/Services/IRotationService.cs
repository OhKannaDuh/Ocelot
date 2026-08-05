namespace Ocelot.Rotation.Services;

public interface IRotationService
{
    void Load()
    {
    }

    void Unload()
    {
    }

    void EnableAutoRotation();

    void DisableAutoRotation();

    void EnableSingleTarget();

    void DisableSingleTarget();

    /// <summary>Create ephemeral AI preset without activating (Illegal Mode on).</summary>
    void EnsureAutoRotationPreset()
    {
    }

    /// <summary>Deactivate and delete ephemeral AI preset (Illegal Mode off).</summary>
    void DestroyAutoRotationPreset()
    {
    }

    /// <summary>Optional keep-alive (e.g. retry BossMod preset IPC once the plugin is ready).</summary>
    void Refresh()
    {
    }
}
