using Dalamud.Bindings.ImGui;
using Ocelot.Graphics;

namespace Ocelot.UI.Services;

public partial class OcelotUIService
{
    public void Text(string text, Color? color = null)
    {
        ImGui.TextColored(ColorToImGui(color), text);
    }

    public void Text(object obj, Color? color = null)
    {
        Text(obj.ToString() ?? "", color);
    }

    public void LabelledValue(string label, string value, Color? labelColor = null, Color? valueColor = null)
    {
        ImGui.TextColored(ColorToImGui(labelColor, branding.DalamudYellow), $"{label}:");
        ImGui.SameLine();
        ImGui.TextColored(ColorToImGui(valueColor), value);
    }

    public void LabelledValue(string label, object value, Color? labelColor = null, Color? valueColor = null)
    {
        LabelledValue(label, value.ToString() ?? "", labelColor, valueColor);
    }

    public void LabelledValue(object label, string value, Color? labelColor = null, Color? valueColor = null)
    {
        LabelledValue(label.ToString() ?? "", value, labelColor, valueColor);
    }

    public  void LabelledValue(object label, object value, Color? labelColor = null, Color? valueColor = null)
    {
        LabelledValue(label.ToString() ?? "", value.ToString() ?? "", labelColor, valueColor);
    }
}
