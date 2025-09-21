namespace Ocelot.Rotation.Services;

public interface IRotationProvider
{
    string InternalName { get; }

    string DisplayName { get; }

    bool IsAvailable();

    IRotationService Create();
}
