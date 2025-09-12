namespace Ocelot.Mechanic.Services;

public interface IMechanicProvider
{
    string InternalName { get; }

    string DisplayName { get; }

    int Priority { get; }

    bool IsAvailable();

    IMechanicService Create();
}
