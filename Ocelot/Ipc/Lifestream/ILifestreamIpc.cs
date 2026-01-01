namespace Ocelot.Ipc.BossMod;

public interface ILifestreamIpc
{
    bool IsBusy();

    uint GetActiveCustomAetheryte();

    bool AethernetTeleportByPlaceNameId(uint placeNameRowId);
}
