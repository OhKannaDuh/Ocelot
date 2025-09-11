using System;
using ECommons.EzIpcManager;

namespace Ocelot.Ipc;

[OcelotIpc("YesAlready")]
public static class YesAlready
{
    [EzIPC]
    public static Func<bool> IsPluginEnabled = null!;

    [EzIPC]
    public static Action<bool> SetPluginEnabled = null!;

    [EzIPC]
    public static Func<string, bool> IsBotherEnabled = null!;

    [EzIPC]
    public static Action<string, bool> SetBotherEnabled = null!;

    [EzIPC]
    public static Action<int> PausePlugin = null!;

    [EzIPC]
    public static Func<string, int, bool> PauseBother = null!;
}
