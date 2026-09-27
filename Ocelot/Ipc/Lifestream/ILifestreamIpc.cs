namespace Ocelot.Ipc.Lifestream;

public interface ILifestreamIpc
{
    bool IsAvailable { get; }

    bool IsBusy();

    uint GetActiveCustomAetheryte();

    bool AethernetTeleportByPlaceNameId(uint placeNameRowId);

    void Abort();
}
