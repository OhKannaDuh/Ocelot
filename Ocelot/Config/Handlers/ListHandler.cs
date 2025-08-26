using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class ListHandler<T> : SelectHandler<T>
    where T : notnull
{
    protected override Type type
    {
        get => typeof(T);
    }

    private readonly IListProvider<T> provider;
    private readonly ModuleConfig selfConfig;
    private readonly PropertyInfo propInfo;

    public ListHandler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop, string provider)
        : base(self, attribute, prop)
    {
        selfConfig = self;
        propInfo = prop;

        if (!string.IsNullOrEmpty(self.ProviderNamespace))
        {
            provider = $"{self.ProviderNamespace}.{provider}";
        }

        var providerType = Registry.GetAllLoadableTypes().FirstOrDefault(t => t.FullName == provider);

        if (providerType == null)
        {
            throw new InvalidOperationException($"Provider type '{provider}' not found.");
        }

        var expectedInterface = typeof(IListProvider<>).MakeGenericType(typeof(T));

        if (!expectedInterface.IsAssignableFrom(providerType))
        {
            var matchingInterfaces = providerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IListProvider<>))
                .Select(i => $"- {i.FullName}")
                .ToList();

            var debugInfo = string.Join("\n", matchingInterfaces);
            throw new InvalidOperationException(
                $"Provider type '{providerType.FullName}' does not implement IListProvider<{typeof(T).Name}>.\n" +
                (matchingInterfaces.Count > 0
                    ? $"Found generic interfaces:\n{debugInfo}"
                    : "No IListProvider<> interfaces were found on the type.")
            );
        }

        try
        {
            this.provider = (IListProvider<T>)Activator.CreateInstance(providerType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create instance of provider '{providerType.FullName}': {ex.Message}", ex);
        }
    }

    protected override IEnumerable<T> GetData()
    {
        return provider.GetItems().Where(provider.Filter);
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
