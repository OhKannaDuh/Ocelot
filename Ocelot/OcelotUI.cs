using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ImGuiNET;

namespace Ocelot;

public class OcelotUI
{
    public static void Title(string title)
    {
        ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), title);
    }

    public static void Error(string error)
    {
        ImGui.TextColored(new Vector4(0.89f, 0.29f, 0.29f, 1f), error);
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

    public static UIState LeftRightText(string left, string right)
    {
        var state = UIState.None;

        ImGui.TextUnformatted(left);
        if (ImGui.IsItemHovered())
        {
            state = UIState.LeftHovered;
        }

        var start = ImGui.GetCursorPosX();
        ImGui.SameLine();
        var width = ImGui.GetCursorPosX() - start;

        var rightSize = ImGui.CalcTextSize(right);
        var avail = ImGui.GetContentRegionAvail();

        var offset = avail.X - rightSize.X;

        if (offset > 0)
        {
            offset += width;
        }

        ImGui.SameLine(Math.Max(0f, offset));
        ImGui.TextUnformatted(right);
        if (ImGui.IsItemHovered())
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

    public static bool IconButtonWithLeftText(
        FontAwesomeIcon icon,
        string text,
        Vector4? defaultColor = null,
        Vector4? activeColor = null,
        Vector4? hoveredColor = null,
        Vector2? size = null)
    {
        using var col = new ImRaii.Color();

        if (defaultColor.HasValue)
        {
            col.Push(ImGuiCol.Button, defaultColor.Value);
        }

        if (activeColor.HasValue)
        {
            col.Push(ImGuiCol.ButtonActive, activeColor.Value);
        }

        if (hoveredColor.HasValue)
        {
            col.Push(ImGuiCol.ButtonHovered, hoveredColor.Value);
        }

        if (size.HasValue)
        {
            size *= ImGuiHelpers.GlobalScale;
        }

        bool button;

        Vector2 iconSize;
        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            iconSize = ImGui.CalcTextSize(icon.ToIconString());
        }

        var textStr = text;
        if (textStr.Contains('#'))
        {
            textStr = textStr[..textStr.IndexOf('#', StringComparison.Ordinal)];
        }

        var framePadding = ImGui.GetStyle().FramePadding;
        var iconPadding = 3 * ImGuiHelpers.GlobalScale;

        var cursor = ImGui.GetCursorScreenPos();

        var textSize = ImGui.CalcTextSize(textStr);

        using (ImRaii.PushId(text))
        {
            var width = size is { X: not 0 } ? size.Value.X : iconSize.X + textSize.X + framePadding.X * 2 + iconPadding;
            var height = size is { Y: not 0 } ? size.Value.Y : ImGui.GetFrameHeight();

            button = ImGui.Button(string.Empty, new Vector2(width, height));
        }

        var textPos = cursor + framePadding;
        var iconPos = new Vector2(textPos.X + textSize.X + iconPadding, cursor.Y + framePadding.Y);

        var dl = ImGui.GetWindowDrawList();

        dl.AddText(textPos, ImGui.GetColorU32(ImGuiCol.Text), textStr);

        using (ImRaii.PushFont(UiBuilder.IconFont))
        {
            dl.AddText(iconPos, ImGui.GetColorU32(ImGuiCol.Text), icon.ToIconString());
        }

        return button;
    }
}
