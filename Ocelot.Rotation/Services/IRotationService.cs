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

    /// <summary>Optional keep-alive (e.g. retry BossMod preset IPC once the plugin is ready).</summary>
    void Refresh()
    {
    }
}
