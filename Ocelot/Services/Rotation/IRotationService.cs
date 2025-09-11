using Ocelot.Gameplay;

namespace Ocelot.Services.Rotation;

public interface IRotationService
{
    IPlugin Plugin { get; }

    void Enable();

    void Disable();
}
