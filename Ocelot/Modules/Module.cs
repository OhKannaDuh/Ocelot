namespace Ocelot.Modules;

public abstract partial class Module<P, C>(P plugin, C pluginConfig) : IModule
    where P : OcelotPlugin
    where C : OcelotConfig
{
    public readonly P Plugin = plugin;

    public readonly C PluginConfig = pluginConfig;

    public virtual bool IsEnabled
    {
        get => true;
    }

    public virtual ModuleConfig? Config
    {
        get => null;
    }

    public virtual void PreDispose()
    {
    }

    public virtual void Dispose()
    {
    }

    public virtual void PostDispose()
    {
    }
}
