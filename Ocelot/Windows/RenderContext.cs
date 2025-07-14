using Ocelot.Commands;
using Ocelot.IPC;
using Ocelot.Modules;

namespace Ocelot.Windows;

public class RenderContext(OcelotPlugin plugin)
{
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