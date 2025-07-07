using System;
using System.Reflection;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class FloatRange : Handler
{
    protected override Type type
    {
        get => typeof(float);
    }

    public FloatRange(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop)
        : base(self, attribute, prop)
    {
    }

    protected override (bool handled, bool changed) RenderComponent(RenderContext payload)
    {
        var value = (float)(payload.GetValue() ?? 1f);

        var attribute = this.attribute as FloatRangeAttribute;
        if (ImGui.SliderFloat(payload.GetLabelWithId(), ref value, attribute!.min, attribute!.max))
        {
            payload.SetValue(value);
            return (true, true);
        }

        return (true, false);
    }
}
