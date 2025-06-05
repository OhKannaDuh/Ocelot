
using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

public class Lifestream : IPCProvider
{
    public Lifestream() : base("Lifestream") { }

    [EzIPC]
    public readonly Func<uint, bool>? AethernetTeleportByPlaceNameId;

    [EzIPC]
    public readonly Func<uint>? GetActiveAetheryte;
}
