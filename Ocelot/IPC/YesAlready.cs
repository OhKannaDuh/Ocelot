using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class YesAlready() : IPCSubscriber("YesAlready")
{
    [EzIPC] public readonly Func<bool> IsPluginEnabled = null!;

    [EzIPC] public readonly Action<bool> SetPluginEnabled = null!;

    [EzIPC] public readonly Func<string, bool> IsBotherEnabled = null!;

    [EzIPC] public readonly Action<string, bool> SetBotherEnabled = null!;

    [EzIPC] public readonly Action<int> PausePlugin = null!;

    [EzIPC] public readonly Func<string, int, bool> PauseBother = null!;
}
