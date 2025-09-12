namespace Ocelot.Rotation.Services;

public interface IRotationService
{
    void Load();

    void Unload();

    void EnableAutoRotation();

    void DisableAutoRotation();
}
