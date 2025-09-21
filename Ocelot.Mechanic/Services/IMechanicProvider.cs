namespace Ocelot.Mechanic.Services;

public interface IMechanicProvider
{
    string InternalName { get; }

    string DisplayName { get; }

    bool IsAvailable();

    IMechanicService Create();
}
