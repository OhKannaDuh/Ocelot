using Dalamud.Plugin.Services;

namespace Ocelot.Modules;

public class UpdateContext(IFramework framework, OcelotPlugin plugin, IModule? module = null)
{
    public readonly IFramework Framework = framework;

    public readonly OcelotPlugin Plugin = plugin;

    public IModule? Module { get; private set; } = module;

    public OcelotConfig Config {
        get => Plugin.OcelotConfig;
    }

    public UpdateContext ForModule(IModule module)
    {
        return new UpdateContext(Framework, Plugin, module);
    }

    public bool IsForModule<T>(out T module) where T : class, IModule
    {
        if (Module is T typed)
        {
            module = typed;
            return true;
        }

        module = null!;
        return false;
    }
}
