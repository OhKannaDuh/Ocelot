using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class ExcelSheetHandler<T> : Handler
    where T : struct, IExcelRow<T>
{
    protected override Type type
    {
        get => typeof(uint);
    }

    private readonly IExcelSheetItemProvider<T> provider;

    public ExcelSheetHandler(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop, string provider)
        : base(self, attribute, prop)
    {
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

        var expectedInterface = typeof(IExcelSheetItemProvider<>).MakeGenericType(typeof(T));

        if (!expectedInterface.IsAssignableFrom(providerType))
        {
            // Provide more helpful debugging info
            var implementedInterfaces = providerType.GetInterfaces();
            var matchingInterfaces = implementedInterfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IExcelSheetItemProvider<>))
                .ToList();

            var debugInfo = string.Join("\n", matchingInterfaces.Select(i => $"- {i.FullName}"));
            throw new InvalidOperationException(
                $"Provider type '{providerType.FullName}' does not implement IExcelSheetItemProvider<{typeof(T).Name}>.\n" +
                $"Found generic interfaces:\n{debugInfo}"
            );
        }

        try
        {
            this.provider = (IExcelSheetItemProvider<T>)Activator.CreateInstance(providerType)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create instance of provider '{providerType.FullName}': {ex.Message}", ex);
        }
    }


    private List<T> GetData()
    {
        return Svc.Data.GetExcelSheet<T>().Where(provider.Filter).ToList();
    }

    protected override (bool handled, bool changed) RenderComponent(RenderContext context)
    {
        var data = GetData();
        var value = (uint)(context.GetValue() ?? 1);
        var selected = Svc.Data.GetExcelSheet<T>().First(d => d.RowId == value);

        var dirty = false;
        if (ImGui.BeginCombo(context.GetLabelWithId(), provider.GetLabel(selected)))
        {
            foreach (var datum in data)
            {
                if (datum.RowId <= 0)
                {
                    continue;
                }

                var isSelected = datum.RowId == value;
                if (ImGui.Selectable(provider.GetLabel(datum), isSelected))
                {
                    context.SetValue(datum.RowId);
                    dirty = true;
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        return (true, dirty);
    }
}
