using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

public class ThatsAWall() : IPCSubscriber("ThatsAWall")
{
    [EzIPC] public readonly Action<int> Pause = null!;
}
