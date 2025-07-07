using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class ThatsAWall : IPCProvider
{
    public ThatsAWall() : base("ThatsAWall")
    {
    }

    [EzIPC] public readonly Action<int> Pause;
}
