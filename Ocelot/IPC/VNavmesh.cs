
using System;
using System.Numerics;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

public class VNavmesh : IPCProvider
{
    public VNavmesh() : base("vnavmesh") { }

    [EzIPC("SimpleMove.PathfindAndMoveTo")]
    public readonly Func<Vector3, bool, bool>? PathfindAndMoveTo;
}
