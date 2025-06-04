using System;
using Dalamud.Plugin.Services;

namespace Ocelot.Modules;

public abstract class Module<P, C> : IDisposable
    where P : OcelotPlugin
    where C : IOcelotConfig
{
    public readonly P plugin;

    public readonly C _config;

    public virtual ModuleConfig? config
    {
        get => null;
    }

    public virtual bool enabled
    {
        get => true;
    }

    public Module(P plugin, C config)
    {
        this.plugin = plugin;
        _config = config;
    }

    public virtual void Initialize() { }

    public virtual void Tick(IFramework _) { }

    public virtual void Render() { }

    public virtual void Dispose() { }
}
