using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Ocelot.Ipc.BossMod;

namespace Ocelot.Ipc.Lifestream;

public class LifestreamIpc(IDalamudPluginInterface plugin) : ILifestreamIpc
{
    private readonly ICallGateSubscriber<bool> isBusy = plugin.GetIpcSubscriber<bool>("Lifestream.IsBusy");

    private readonly ICallGateSubscriber<uint> getActiveCustomAetheryte = plugin.GetIpcSubscriber<uint>("Lifestream.GetActiveCustomAetheryte");

    private readonly ICallGateSubscriber<uint, bool> aethernetTeleportByPlaceNameId = plugin.GetIpcSubscriber<uint, bool>("Lifestream.AethernetTeleportByPlaceNameId");

    public bool IsBusy()
    {
        return isBusy.InvokeFunc();
    }

    public uint GetActiveCustomAetheryte()
    {
        return getActiveCustomAetheryte.InvokeFunc();
    }

    public bool AethernetTeleportByPlaceNameId(uint placeNameRowId)
    {
        return aethernetTeleportByPlaceNameId.InvokeFunc(placeNameRowId);
    }
}
