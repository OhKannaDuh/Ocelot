using System;
using System.Collections.Generic;
using System.Reflection;
using ECommons;
using Ocelot.Config.Handlers;
using Ocelot.Modules;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class EnumOrderSelectAttribute : ConfigAttribute
{
    private readonly Type enumType;

    private readonly string providerName;

    public EnumOrderSelectAttribute(Type type, string provider)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (provider.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(provider));
        }

        if (!type.IsEnum)
        {
            throw new ArgumentException($"Type '{type.FullName}' must be an enum.", nameof(type));
        }

        enumType = type;
        providerName = provider;
    }

    public override Handler GetHandler(ModuleConfig self, ConfigAttribute attr, PropertyInfo prop)
    {
        var ok =
            prop.PropertyType.IsArray && prop.PropertyType.GetElementType() == enumType ||
            prop.PropertyType.IsGenericType &&
            prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
            prop.PropertyType.GetGenericArguments()[0] == enumType;

        if (!ok)
        {
            throw new ArgumentException(
                $"Property '{prop.Name}' must be List<{enumType.Name}> or {enumType.Name}[] for [OrderSelect]. " +
                $"Actual: '{prop.PropertyType.FullName}'.");
        }

        var handlerType = typeof(EnumOrderSelectHandler<>).MakeGenericType(enumType);
        return (Handler)Activator.CreateInstance(handlerType, self, attr, prop, providerName)!;
    }
}
