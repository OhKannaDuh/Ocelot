using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class EnumHandler<T> : Handler
    where T : Enum
{
    protected override Type type
    {
        get => typeof(T);
    }

    private readonly IEnumProvider<T> provider;

    private readonly bool isSearchable;

    private string searchTerm = "";

    public EnumHandler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop, string provider)
        : base(self, attribute, prop)
    {
        isSearchable = prop.IsDefined(typeof(SearchableAttribute), true);

        if (!string.IsNullOrEmpty(self.ProviderNamespace))
        {
            provider = $"{self.ProviderNamespace}.{provider}";
        }

        // Use your Registry to find the type
        var providerType = Registry.GetAllLoadableTypes()
            .FirstOrDefault(t => t.FullName == provider);

        if (providerType == null)
        {
            throw new InvalidOperationException($"Provider type '{provider}' not found.");
        }

        var expectedInterface = typeof(IEnumProvider<>).MakeGenericType(typeof(T));

        if (!expectedInterface.IsAssignableFrom(providerType))
        {
            // Provide more helpful debugging info
            var implementedInterfaces = providerType.GetInterfaces();
            var matchingInterfaces = implementedInterfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumProvider<>))
                .ToList();

            var debugInfo = string.Join("\n", matchingInterfaces.Select(i => $"- {i.FullName}"));
            throw new InvalidOperationException(
                $"Provider type '{providerType.FullName}' does not implement IEnumProvider<{typeof(T).Name}>.\n" +
                $"Found generic interfaces:\n{debugInfo}"
            );
        }

        try
        {
            this.provider = (IEnumProvider<T>)Activator.CreateInstance(providerType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create instance of provider '{providerType.FullName}': {ex.Message}", ex);
        }
    }


    protected override (bool handled, bool changed) RenderComponent(RenderContext context)
    {
        var currentValue = (T)context.GetValue()!;

        var dirty = false;
        if (ImGui.BeginCombo(context.GetLabelWithId(), provider.GetLabel(currentValue), ImGuiComboFlags.HeightLarge))
        {
            if (isSearchable)
            {
                ImGui.InputText("##search", ref searchTerm, 256);
                ImGui.Separator();
            }

            var values = GetValuesToShow().ToList();
            var height = Math.Min(512, values.Count * ImGui.GetTextLineHeightWithSpacing());

            using (ImRaii.Child("##options", new Vector2(0, height)))
            {
                foreach (var value in values)
                {
                    var isSelected = EqualityComparer<T>.Default.Equals(value, currentValue);
                    if (ImGui.Selectable(provider.GetLabel(value), isSelected))
                    {
                        context.SetValue(value);
                        dirty = true;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
            }

            ImGui.EndCombo();
        }

        return (true, dirty);
    }

    private IEnumerable<T> GetValuesToShow()
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (!provider.Filter(value) || !SearchFilter(value))
            {
                continue;
            }

            yield return value;
        }
    }

    private bool SearchFilter(T item)
    {
        if (!isSearchable || searchTerm.Trim() == string.Empty)
        {
            return true;
        }

        return provider.GetLabel(item).Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }
}
