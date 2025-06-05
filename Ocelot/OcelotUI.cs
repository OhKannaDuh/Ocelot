using System;
using System.Numerics;
using ECommons.DalamudServices;
using ImGuiNET;

namespace Ocelot;

public class OcelotUI
{
    public static void Title(string title) => ImGui.TextColored(new Vector4(1f, 0.75f, 0.25f, 1f), title);


    public static void Region(string id, Action contents)
    {
        ImGui.PushID(id);
        contents();
        ImGui.PopID();
    }

    public static void Indent(uint depth, Action contents)
    {
        ImGui.Indent(depth);
        contents();
        ImGui.Unindent(depth);
    }

    public static void DrawLine(Vector3 start, Vector3 end, float thickness, Vector4 color)
    {
        bool startValid = Svc.GameGui.WorldToScreen(start, out Vector2 startScreen);
        bool endValid = Svc.GameGui.WorldToScreen(end, out Vector2 endScreen);

        if (startValid && endValid)
        {
            var imguiColor = ImGui.ColorConvertFloat4ToU32(color);
            ImGui.GetBackgroundDrawList().AddLine(startScreen, endScreen, imguiColor, thickness);
        }
    }

    public static void Separator()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        var pos = ImGui.GetCursorScreenPos();
        var width = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
        var drawList = ImGui.GetWindowDrawList();

        drawList.AddLine(new Vector2(pos.X, pos.Y), new Vector2(pos.X + width, pos.Y), ImGui.GetColorU32(ImGuiCol.Separator));

        ImGui.Dummy(new Vector2(width, 1));

        ImGui.PopStyleVar();
    }

    public static void VSpace(int px = 8)
    {
        ImGui.Dummy(new Vector2(0, px));
    }
}
