
using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class Lifestream : IPCProvider
{
    public Lifestream() : base("Lifestream") { }

    [EzIPC]
    public readonly Func<uint, bool> AethernetTeleportByPlaceNameId;

    [EzIPC]
    public readonly Func<uint> GetActiveAetheryte;
}
