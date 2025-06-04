using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Plugin.Services;
using ECommons.Reflection;

namespace Ocelot.Modules;

public class ModuleManager<P, C>
    where P : OcelotPlugin
    where C : IOcelotConfig
{
    private readonly List<Module<P, C>> modules = new();

    private List<Module<P, C>> enabled => modules.Where(m => m.enabled).ToList();

    public void Add(Module<P, C> module) => modules.Add(module);

    public void AutoRegister(P plugin, C config)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }
            catch
            {
                // Could not load types from this assembly, skip
                continue;
            }

            foreach (var type in types)
            {
                if (type.GetCustomAttribute<OcelotModuleAttribute>() != null && typeof(Module<P, C>).IsAssignableFrom(type))
                {
                    // Create instance and add to list
                    var moduleInstance = (Module<P, C>)Activator.CreateInstance(type, plugin, config);
                    Logger.Info($"Registering module: {type.FullName}");
                    modules.Add(moduleInstance);
                }
            }
        }
    }

    public void Initialize(P plugin, C config)
    {
        AutoRegister(plugin, config);
        enabled.ForEach(m => m.Initialize());
    }

    public void Tick(IFramework framework) => enabled.ForEach(m => m.Tick(framework));

    public void Render()
    {
        Logger.Info($"Enabled modules: {enabled.Count}");
        enabled.ForEach(m => m.Render());
    }

    public void RenderConfig()
    {
        foreach (var module in enabled)
        {
            var config = module.config;
            if (config != null)
            {
                if (config.Draw())
                {
                    module._config.Save();
                }
            }
        }
    }

    public void Dispose() => modules.ForEach(m => m.Dispose());
}
