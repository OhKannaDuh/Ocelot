
using System;
using ECommons.EzIpcManager;

namespace Ocelot.IPC;

#pragma warning disable CS8618
public class YesAlready : IPCProvider
{
    public YesAlready() : base("YesAlready") { }

    [EzIPC]
    public readonly Func<bool> IsPluginEnabled;

    [EzIPC]
    public readonly Action<bool> SetPluginEnabled;

    [EzIPC]
    public readonly Func<string, bool> IsBotherEnabled;

    [EzIPC]
    public readonly Action<string, bool> SetBotherEnabled;

    [EzIPC]
    public readonly Action<int> PausePlugin;

    [EzIPC]
    public readonly Func<string, int, bool> PauseBother;
}
