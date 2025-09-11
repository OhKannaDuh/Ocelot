using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.Reflection;

namespace Ocelot.Gameplay.Mechanic;

public static class MechanicHelper
{
    private static bool Initialized = false;

    private static readonly Dictionary<string, IMechanicPlugin> Plugins = [];

    internal static void Initialize(OcelotPlugin plugin)
    {
        if (Initialized)
        {
            return;
        }

        Logger.Info("[MechanicHelper] Initializing");
        Initialized = true;

        var types = Registry.GetTypesImplementing<IMechanicPlugin>().Where(t => !t.IsAbstract);
        foreach (var type in types)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null || Activator.CreateInstance(type) is not IMechanicPlugin instance)
            {
                continue;
            }

            if (instance.InternalName != "None")
            {
                Logger.Info($"[MechanicHelper] Registering Mechanic Plugin '{instance.InternalName}'");
            }

            Plugins.Add(instance.InternalName, instance);
        }
    }

    internal static void TearDown()
    {
        if (!Initialized)
        {
            return;
        }

        foreach (var (name, plugin) in Plugins)
        {
            plugin.Dispose();
            Plugins.Remove(name);
        }
    }

    public static IMechanicPlugin GetPlugin()
    {
        if (!Initialized)
        {
            throw new MechanicHelperNotInitializedException();
        }

        foreach (var (name, mechanicPlugin) in Plugins)
        {
            if (!DalamudReflector.TryGetDalamudPlugin(name, out _, false, true))
            {
                continue;
            }

            return mechanicPlugin;
        }

        return Plugins["None"];
    }

    public static IMechanicPlugin GetPlugin(string name)
    {
        if (!Initialized)
        {
            throw new MechanicHelperNotInitializedException();
        }

        if (Plugins.ContainsKey(name) && DalamudReflector.TryGetDalamudPlugin(name, out _, false, true))
        {
            return Plugins[name];
        }

        return Plugins["None"];
    }
}
