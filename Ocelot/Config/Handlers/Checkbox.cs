using System;
using System.Reflection;
using ImGuiNET;
using Ocelot.Config.Attributes;
using Ocelot.Modules;

namespace Ocelot.Config.Handlers;

public class Checkbox : Handler
{
    protected override Type type => typeof(bool);

    public Checkbox(ModuleConfig self, ConfigAttribute attribute, PropertyInfo prop)
        : base(self, attribute, prop) { }

    protected override (bool handled, bool changed) RenderComponent(RenderContext payload)
    {
        bool value = (bool)(payload.GetValue() ?? false);

        if (ImGui.Checkbox(payload.GetLabelWithId(), ref value))
        {
            payload.SetValue(value);
            return (true, true);
        }

        return (true, false);
    }
}
