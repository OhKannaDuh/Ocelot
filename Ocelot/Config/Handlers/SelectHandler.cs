﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public abstract class SelectHandler<T>(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop) : Handler(self, attribute, prop)
    where T : notnull
{
    protected override Type type {
        get => typeof(T);
    }

    private readonly bool isSearchable = prop.IsDefined(typeof(SearchableAttribute), true);

    private string searchTerm = "";

    protected override (bool handled, bool changed) RenderComponent(RenderContext context)
    {
        var currentValue = GetValue(context);

        var dirty = false;
        if (ImGui.BeginCombo(context.GetLabelWithId(), GetLabel(currentValue), ImGuiComboFlags.HeightLarge))
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
                    if (ImGui.Selectable(GetLabel(value), isSelected))
                    {
                        SetValue(context, value);
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
        foreach (var value in GetData())
        {
            if (!Filter(value) || !SearchFilter(value))
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

        return GetLabel(item).Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }

    protected virtual T GetValue(RenderContext context)
    {
        return (T)context.GetValue()!;
    }

    protected virtual void SetValue(RenderContext context, T value)
    {
        context.SetValue(value);
    }

    protected abstract IEnumerable<T> GetData();

    protected abstract bool Filter(T item);

    protected abstract string GetLabel(T item);
}
