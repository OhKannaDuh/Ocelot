using System;
using System.Reflection;
using Ocelot.Config.Handlers;
using Ocelot.Modules;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ListAttribute(Type elementType, string provider) : ConfigAttribute
{
    private Type ElementType { get; } = elementType ?? throw new ArgumentNullException(nameof(elementType));

    private readonly string provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public override Handler GetHandler(ModuleConfig self, ConfigAttribute attr, PropertyInfo prop)
    {
        if (prop.PropertyType != ElementType)
        {
            throw new ArgumentException(
                $"Property '{prop.Name}' must be of type '{ElementType.FullName}' for [List(typeof({ElementType.Name}), ...)]. " +
                $"Actual: '{prop.PropertyType.FullName}'.");
        }

        var handlerType = typeof(ListHandler<>).MakeGenericType(ElementType);
        return (Handler)Activator.CreateInstance(handlerType, self, attr, prop, provider)!;
    }
}
