
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class VNavmesh : IPCProvider
{
    public VNavmesh() : base("vnavmesh") { }

    [EzIPC("SimpleMove.PathfindAndMoveTo")]
    public readonly Func<Vector3, bool, bool> PathfindAndMoveTo;

    [EzIPC("Path.MoveTo")]
    public readonly Action<List<Vector3>, bool> FollowPath;

    [EzIPC("Path.IsRunning")]
    public readonly Func<bool> IsRunning;

    public async Task WaitToStop()
    {
        while (true)
        {
            if (IsRunning == null)
                break;

            if (!IsRunning())
                break;

            await Task.Delay(50);
        }
    }

    public async Task WaitToStart()
    {
        while (true)
        {
            if (IsRunning == null)
                break;

            if (IsRunning())
                break;

            await Task.Delay(50);
        }
    }

}
