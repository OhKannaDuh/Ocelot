using Ocelot.Gameplay;

namespace Ocelot.Services.Mechanic;

public interface IMechanicService
{
    IPlugin Plugin { get; }

    void Enable();

    void Disable();
}
