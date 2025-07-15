using Dalamud.Plugin.Services;
using Ocelot.Commands;
using Ocelot.IPC;
using Ocelot.Windows;

namespace Ocelot.Modules;

public class UpdateContext(IFramework framework, OcelotPlugin plugin)
{
    public readonly IFramework Framework = framework;

    public readonly OcelotPlugin Plugin = plugin;

    public IOcelotConfig Config
    {
        get => Plugin.OcelotConfig;
    }

    public ModuleManager Modules
    {
        get => Plugin.Modules;
    }

    public WindowManager Windows
    {
        get => Plugin.Windows;
    }

    public CommandManager Commands
    {
        get => Plugin.Commands;
    }

    public IPCManager IPC
    {
        get => Plugin.IPC;
    }
}
