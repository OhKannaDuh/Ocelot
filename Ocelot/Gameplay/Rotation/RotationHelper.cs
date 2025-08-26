using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.Reflection;

namespace Ocelot.Gameplay.Rotation;

public static class RotationHelper
{
    private static bool Initialized = false;

    private readonly static Dictionary<string, IRotationPlugin> Plugins = [];

    internal static void Initialize(OcelotPlugin plugin)
    {
        if (Initialized)
        {
            return;
        }

        Logger.Info("[RotationHelper] Initializing");
        Initialized = true;

        var types = Registry.GetTypesImplementing<IRotationPlugin>().Where(t => !t.IsAbstract);
        foreach (var type in types)
        {
            var ctor = type.GetConstructor([typeof(OcelotPlugin)]);
            if (ctor == null || Activator.CreateInstance(type, plugin) is not IRotationPlugin instance)
            {
                continue;
            }

            if (instance.InternalName != "None")
            {
                Logger.Info($"[RotationHelper] Registering Rotation Plugin '{instance.InternalName}'");
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

    public static IRotationPlugin GetPlugin()
    {
        if (!Initialized)
        {
            throw new RotationHelperNotInitializedException();
        }

        foreach (var (name, rotationPlugin) in Plugins)
        {
            if (!DalamudReflector.TryGetDalamudPlugin(name, out _, false, true))
            {
                continue;
            }

            return rotationPlugin;
        }

        return Plugins["None"];
    }

    public static IRotationPlugin GetPlugin(string name)
    {
        if (!Initialized)
        {
            throw new RotationHelperNotInitializedException();
        }

        if (Plugins.ContainsKey(name) && DalamudReflector.TryGetDalamudPlugin(name, out _, false, true))
        {
            return Plugins[name];
        }

        return Plugins["None"];
    }
}
