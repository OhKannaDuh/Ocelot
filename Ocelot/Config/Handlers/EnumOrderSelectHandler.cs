using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Bindings.ImGui;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class EnumOrderSelectHandler<TEnum> : Handler
    where TEnum : struct, Enum
{
    protected override Type type
    {
        get => typeof(List<TEnum>);
    }

    private readonly ModuleConfig selfRef;
    private readonly bool bindsArray;
    private readonly IEnumProvider<TEnum> provider;

    public EnumOrderSelectHandler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo property, string providerName)
        : base(self, attribute, property)
    {
        selfRef = self;
        bindsArray = property.PropertyType.IsArray;

        var qualifiedName = string.IsNullOrEmpty(self.ProviderNamespace)
            ? providerName
            : $"{self.ProviderNamespace}.{providerName}";

        var providerType = Registry.GetAllLoadableTypes()
                               .FirstOrDefault(t => t.FullName == qualifiedName)
                           ?? throw new InvalidOperationException($"Provider type '{qualifiedName}' not found.");

        var expected = typeof(IEnumProvider<>).MakeGenericType(typeof(TEnum));
        if (!expected.IsAssignableFrom(providerType))
        {
            var matches = providerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumProvider<>))
                .Select(i => $"- {i.FullName}");
            var debugInfo = string.Join("\n", matches);
            throw new InvalidOperationException(
                $"Provider type '{providerType.FullName}' does not implement IEnumProvider<{typeof(TEnum).Name}>.\n" +
                (string.IsNullOrEmpty(debugInfo)
                    ? "No IEnumProvider<> interfaces were found on the type."
                    : $"Found generic interfaces:\n{debugInfo}")
            );
        }

        try
        {
            provider = (IEnumProvider<TEnum>)Activator.CreateInstance(providerType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create instance of provider '{providerType.FullName}': {ex.Message}", ex);
        }
    }

    private List<TEnum> ReadOrder()
    {
        var val = property.GetValue(selfRef);
        if (val is null)
        {
            return new List<TEnum>();
        }

        return bindsArray ? ((TEnum[])val).ToList() : (List<TEnum>)val;
    }

    private void WriteOrder(List<TEnum> order)
    {
        if (bindsArray)
        {
            property.SetValue(selfRef, order.ToArray());
        }
        else
        {
            property.SetValue(selfRef, order);
        }
    }

    private static List<TEnum> AllFiltered(IEnumProvider<TEnum> p)
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Where(p.Filter).ToList();
    }

    /// <summary>
    /// 1) Filter out disallowed values
    /// 2) Remove duplicates (preserve first occurrence)
    /// 3) Append any missing allowed values (so each appears once)
    /// </summary>
    private static List<TEnum> Normalize(List<TEnum> raw, IEnumProvider<TEnum> p)
    {
        var allowed = AllFiltered(p);

        // Fast lookup for allowed set
        var allowedSet = new HashSet<TEnum>(allowed);
        var seen = new HashSet<TEnum>();

        var normalized = new List<TEnum>(allowed.Count);

        // keep first occurrence only, skip disallowed
        foreach (var v in raw)
        {
            if (!allowedSet.Contains(v))
            {
                continue;
            }

            if (seen.Add(v))
            {
                normalized.Add(v);
            }
        }

        // append any missing allowed values in enum order
        foreach (var v in allowed)
        {
            if (seen.Add(v))
            {
                normalized.Add(v);
            }
        }

        return normalized;
    }

    protected override (bool handled, bool changed) RenderComponent(RenderContext ctx)
    {
        var changed = false;

        var original = ReadOrder();
        var current = Normalize(original, provider);

        if (!SequenceEqual(original, current))
        {
            WriteOrder(current);
            changed = true;
        }

        ImGui.TextUnformatted(ctx.GetLabel());
        ImGui.Spacing();

        for (var i = 0; i < current.Count; i++)
        {
            var item = current[i];
            var label = provider.GetLabel(item);

            ImGui.PushID($"{property.Name}#{i}");

            // Up
            var canUp = i > 0;
            if (!canUp)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.SmallButton("▲"))
            {
                (current[i - 1], current[i]) = (current[i], current[i - 1]);
                WriteOrder(current);
                changed = true;
            }

            if (!canUp)
            {
                ImGui.EndDisabled();
            }

            ImGui.SameLine();

            // Down
            var canDown = i < current.Count - 1;
            if (!canDown)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.SmallButton("▼"))
            {
                (current[i + 1], current[i]) = (current[i], current[i + 1]);
                WriteOrder(current);
                changed = true;
            }

            if (!canDown)
            {
                ImGui.EndDisabled();
            }

            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(label);

            ImGui.PopID();
        }

        return (true, changed);
    }

    private static bool SequenceEqual(List<TEnum> a, List<TEnum> b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Count; i++)
        {
            if (!EqualityComparer<TEnum>.Default.Equals(a[i], b[i]))
            {
                return false;
            }
        }

        return true;
    }
}
