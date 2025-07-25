using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class MultiEnumHandler<T> : MultiSelectHandler<T>
    where T : Enum 
{
    private readonly IEnumProvider<T> provider;

    public MultiEnumHandler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop, string provider)
        : base(self, attribute, prop) 
    {
        if (!string.IsNullOrEmpty(self.ProviderNamespace))
        {
            provider = $"{self.ProviderNamespace}.{provider}";
        }

        var providerType = Registry.GetAllLoadableTypes().FirstOrDefault(t => t.FullName == provider);

        if (providerType == null)
        {
            throw new InvalidOperationException($"Provider type '{provider}' not found for MultiEnumHandler.");
        }

        var expectedInterface = typeof(IEnumProvider<>).MakeGenericType(typeof(T));

        if (!expectedInterface.IsAssignableFrom(providerType))
        {
            var implementedInterfaces = providerType.GetInterfaces();
            var matchingInterfaces = implementedInterfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumProvider<>))
                .ToList();

            var debugInfo = string.Join("\n", matchingInterfaces.Select(i => $"- {i.FullName}"));
            throw new InvalidOperationException(
                $"Provider type '{providerType.FullName}' does not implement IEnumProvider<{typeof(T).Name}> for MultiEnumHandler.\n" +
                $"Found generic interfaces:\n{debugInfo}"
            );
        }

        try
        {
            this.provider = (IEnumProvider<T>)Activator.CreateInstance(providerType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create instance of provider '{providerType.FullName}' for MultiEnumHandler: {ex.Message}", ex);
        }
    }


    protected override IEnumerable<T> GetData()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }


    protected override bool Filter(T item)
    {
        return provider.Filter(item);
    }

    protected override string GetLabel(T item)
    {
        return provider.GetLabel(item);
    }
}