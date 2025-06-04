using System;
using System.Collections.Generic;
using System.Linq;
using ECommons.DalamudServices;
using ImGuiNET;
using Lumina.Excel;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class ExcelSheet<T> : Handler
    where T : struct, IExcelRow<T>
{
    protected override Type type => typeof(bool);

    private readonly IExcelSheetItemProvider<T> provider;

    public ExcelSheet(ModuleConfig self, ConfigAttribute attribute, string provider)
        : base(self, attribute)
    {
        if (self.ProviderNamespace != "")
        {
            provider = $"{self.ProviderNamespace}.{provider}";
        }

        var providerType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(provider)).FirstOrDefault(t => t != null);

        if (providerType == null)
        {
            throw new InvalidOperationException($"Provider type '{provider}' not found.");
        }

        var expectedInterface = typeof(IExcelSheetItemProvider<>).MakeGenericType(typeof(T));
        if (!expectedInterface.IsAssignableFrom(providerType))
        {
            throw new InvalidOperationException($"Provider type '{provider}' does not implement IExcelSheetItemProvider<{typeof(T).Name}>.");
        }

        this.provider = (IExcelSheetItemProvider<T>)Activator.CreateInstance(providerType)!;
    }

    private List<T> GetData() => Svc.Data.GetExcelSheet<T>().Where(provider.Filter).ToList();

    protected override (bool handled, bool changed) RenderComponent(RenderContext context)
    {
        var data = GetData();
        uint value = (uint)(context.GetValue() ?? 1);
        var selected = data.FirstOrDefault(d => d.RowId == value);

        bool dirty = false;
        if (ImGui.BeginCombo(context.GetLabelWithId(), provider.GetLabel(selected)))
        {
            foreach (var datum in data)
            {
                if (datum.RowId <= 0)
                {
                    continue;
                }

                bool isSelected = datum.RowId == value;
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
