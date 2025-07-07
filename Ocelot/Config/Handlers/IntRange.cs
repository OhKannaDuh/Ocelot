using System;
using System.Reflection;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class IntRange : Handler
{
    protected override Type type
    {
        get => typeof(int);
    }

    public IntRange(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop)
        : base(self, attribute, prop)
    {
    }

    protected override (bool handled, bool changed) RenderComponent(RenderContext payload)
    {
        var value = (int)(payload.GetValue() ?? 1f);

        var attribute = this.attribute as IntRangeAttribute;
        if (ImGui.SliderInt(payload.GetLabelWithId(), ref value, attribute!.min, attribute!.max))
        {
            payload.SetValue(value);
            return (true, true);
        }

        return (true, false);
    }
}
