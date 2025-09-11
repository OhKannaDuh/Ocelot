using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ocelot.States;

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

                    CachedTypes.AddRange(types.Where(t => t != null));
                }
                catch { }
            }
        }
    }

    public static IEnumerable<Type> GetAllLoadableTypes()
    {
        return CachedTypes;
    }

    public static IEnumerable<Type> GetTypesWithAttribute<TAttr>() where TAttr : Attribute
    {
        return GetAllLoadableTypes().Where(t => t.GetCustomAttribute<TAttr>() != null);
    }

    public static IEnumerable<(Type type, TAttr? attr)> GetTypesWithAttributeData<TAttr>() where TAttr : Attribute
    {
        return GetAllLoadableTypes()
            .Select(t => (type: t, attr: t.GetCustomAttribute<TAttr>()))
            .Where(t => t.attr != null);
    }

    public static IEnumerable<Type> GetTypesImplementing<TBase>()
    {
        return GetAllLoadableTypes()
            .Where(t => !t.IsAbstract && typeof(TBase).IsAssignableFrom(t));
    }

    public static IEnumerable<Type> GetTypesForStateMachine<TState, TContext>()
        where TState : struct, Enum
        where TContext : class?
    {
        return GetAllLoadableTypes()
            .Where(t =>
                typeof(StateHandler<TState, TContext>).IsAssignableFrom(t) /* || typeof(ScoreStateHandler<T, M, C>).IsAssignableFrom(t*)*/ &&
                !t.IsAbstract &&
                t.GetCustomAttribute<StateAttribute<TState>>() is not null);
    }
}
