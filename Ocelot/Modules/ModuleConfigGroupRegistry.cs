using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ocelot.Modules;

public static class ModuleConfigGroupRegistry
{
    private static bool Initialized = false;

    private static readonly Dictionary<string, ModuleConfigGroup> Groups = new();

    public static void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        Initialized = true;
        var types = Registry.GetTypesWithAttribute<OcelotConfigGroupsAttribute>();

        foreach (var type in types)
        {
            foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (f.FieldType != typeof(ModuleConfigGroup))
                {
                    continue;
                }

                if (f.GetValue(null) is ModuleConfigGroup group)
                {
                    Register(group);
                }
            }

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (p.PropertyType != typeof(ModuleConfigGroup))
                {
                    continue;
                }

                var getter = p.GetGetMethod(true);

                if (getter?.Invoke(null, null) is ModuleConfigGroup group)
                {
                    Register(group);
                }
            }
        }
    }

    public static ModuleConfigGroup Get(string id)
    {
        return Groups.TryGetValue(id, out var g) ? g : ModuleConfigGroup.Default;
    }

    public static IEnumerable<string> GetConfigGroupOrder()
    {
        return Groups.OrderByDescending(g => g.Value.Priority).Select(g => g.Key);
    }

    private static void Register(ModuleConfigGroup group)
    {
        Logger.Info($"Registering module config group {group.Id}");
        Groups.Add(group.Id, group);
    }
}
