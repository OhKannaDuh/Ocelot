using System;
using System.Numerics;
using ECommons.DalamudServices;
using ImGuiNET;

namespace Ocelot;

public static class OcelotUI
{
    public static void Title(string title)
    {
        ImGui.TextColored(OcelotColor.Yellow, title);
    }

    public static void Error(string error)
    {
        ImGui.TextColored(OcelotColor.Error, error);
    }

    public static UIState LabelledValue(string title, object value)
    {
        return LabelledValue(title, value.ToString() ?? "");
    }

    public static UIState LabelledValue(string title, string value)
    {
        var hovered = false;

        Title($"{title}:");
        hovered |= ImGui.IsItemHovered();
        ImGui.SameLine();
        hovered |= ImGui.IsItemHovered();
        ImGui.TextUnformatted(value);
        hovered |= ImGui.IsItemHovered();

        return hovered ? UIState.Hovered : UIState.None;
    }

    public static UIState LeftRightText(UIString left, UIString right)
    {
        var state = UIState.None;
        if (left.Render() == UIState.Hovered)
        {
            state = UIState.LeftHovered;
        }

        ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - right.Width);
        if (right.Render() == UIState.Hovered)
        {
            state = UIState.RightHovered;
        }

        return state;
    }

    public static void Region(string id, Action contents)
    {
        ImGui.PushID(id);
        contents();
        ImGui.PopID();
    }

    public static void Indent(uint depth, Action contents)
    {
        if (depth > 0)
        {
            ImGui.Indent(depth);
        }

        contents();

        if (depth > 0)
        {
            ImGui.Unindent(depth);
        }
    }

    public static void Indent(Action contents)
    {
        Indent(16, contents);
    }

    public static void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        var startValid = Svc.GameGui.WorldToScreen(start, out var startScreen);
        var endValid = Svc.GameGui.WorldToScreen(end, out var endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetBackgroundDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }

    public static void Separator()
    {
        // ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        var pos = ImGui.GetCursorScreenPos();
        var width = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
        var drawList = ImGui.GetWindowDrawList();

        drawList.AddLine(new Vector2(pos.X, pos.Y), new Vector2(pos.X + width, pos.Y), ImGui.GetColorU32(ImGuiCol.Separator));

        ImGui.Dummy(new Vector2(width, 1));

        // ImGui.PopStyleVar();
    }

    public static void VSpace(int px = 8)
    {
        ImGui.Dummy(new Vector2(0, px));
    }

    public static UIState ProgressBar(float fraction, float rounding = 4.0f)
    {
        return ProgressBar(fraction, new Vector2(ImGui.GetContentRegionAvail().X, 8f), rounding);
    }

    public static UIState ProgressBar(float fraction, Vector2 size, float rounding = 4.0f)
    {
        fraction = Math.Clamp(fraction, 0.0f, 1.0f);

        var drawList = ImGui.GetWindowDrawList();
        var cursorPos = ImGui.GetCursorScreenPos();

        var bgColor = ImGui.GetColorU32(ImGuiCol.FrameBg);
        var fgColor = ImGui.GetColorU32(ImGuiCol.PlotHistogram);

        var barEnd = new Vector2(cursorPos.X + size.X, cursorPos.Y + size.Y);
        var filledEnd = new Vector2(cursorPos.X + size.X * fraction, barEnd.Y);

        drawList.AddRectFilled(cursorPos, barEnd, bgColor, rounding);

        if (fraction > 0.0f)
        {
            drawList.AddRectFilled(cursorPos, filledEnd, fgColor, rounding, ImDrawFlags.RoundCornersLeft);
        }

        ImGui.Dummy(size);

        return ImGui.IsItemHovered() ? UIState.Hovered : UIState.None;
    }
}
