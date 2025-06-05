using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ocelot;

public static class Registry
{
    private static readonly HashSet<Assembly> RegisteredAssemblies = new();
    private static readonly List<Type> CachedTypes = new();

    public static void RegisterAssemblies(params Assembly[] assemblies)
    {
        foreach (var asm in assemblies)
        {
            if (RegisteredAssemblies.Add(asm))
            {
                try
                {
                    Type[] types;
                    try
                    {
                        types = asm.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types.Where(t => t != null).ToArray()!;
                    }

                    foreach (var type in types.Where(t => t != null))
                    {
                        Logger.Debug($"[Registry] Registered type: {type.FullName}");
                    }

                    CachedTypes.AddRange(types.Where(t => t != null));
                }
                catch
                {
                }
            }
        }
    }

    public static IEnumerable<Type> GetAllLoadableTypes()
        => CachedTypes;

    public static IEnumerable<Type> GetTypesWithAttribute<TAttr>() where TAttr : Attribute
        => GetAllLoadableTypes().Where(t => t.GetCustomAttribute<TAttr>() != null);

    public static IEnumerable<(Type type, TAttr attr)> GetTypesWithAttributeData<TAttr>() where TAttr : Attribute
        => GetAllLoadableTypes()
            .Select(t => (type: t, attr: t.GetCustomAttribute<TAttr>()))
            .Where(t => t.attr != null);

    public static IEnumerable<Type> GetTypesImplementing<TBase>()
        => GetAllLoadableTypes()
            .Where(t => !t.IsAbstract && typeof(TBase).IsAssignableFrom(t));

}
