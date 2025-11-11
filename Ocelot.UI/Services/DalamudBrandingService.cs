using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Ocelot.Graphics;
using Ocelot.Services.UI;

namespace Ocelot.UI.Services;

public sealed class DalamudBrandingService : IBrandingService
{
    private readonly Lazy<ImGuiStylePtr> style = new(ImGui.GetStyle);

    private ImGuiStylePtr Style
    {
        get => style.Value;
    }

    public Vector2 WindowPadding
    {
        get => Style.WindowPadding;
    }

    public Vector2 FramePadding
    {
        get => Style.FramePadding;
    }

    public Vector2 CellPadding
    {
        get => Style.CellPadding;
    }

    public Vector2 ItemSpacing
    {
        get => Style.ItemSpacing;
    }

    public Vector2 ItemInnerSpacing
    {
        get => Style.ItemInnerSpacing;
    }

    public float IndentSpacing
    {
        get => Style.IndentSpacing;
    }

    public float ScrollbarSize
    {
        get => Style.ScrollbarSize;
    }

    public float GrabMinSize
    {
        get => Style.GrabMinSize;
    }

    public float WindowBorderSize
    {
        get => Style.WindowBorderSize;
    }

    public float ChildBorderSize
    {
        get => Style.ChildBorderSize;
    }

    public float PopupBorderSize
    {
        get => Style.PopupBorderSize;
    }

    public float FrameBorderSize
    {
        get => Style.FrameBorderSize;
    }

    public float TabBorderSize
    {
        get => Style.TabBorderSize;
    }

    public float WindowRounding
    {
        get => Style.WindowRounding;
    }

    public float ChildRounding
    {
        get => Style.ChildRounding;
    }

    public float FrameRounding
    {
        get => Style.FrameRounding;
    }

    public float PopupRounding
    {
        get => Style.PopupRounding;
    }

    public float ScrollbarRounding
    {
        get => Style.ScrollbarRounding;
    }

    public float GrabRounding
    {
        get => Style.GrabRounding;
    }

    public float TabRounding
    {
        get => Style.TabRounding;
    }

    public Vector2 WindowTitleAlign
    {
        get => Style.WindowTitleAlign;
    }

    public Vector2 ButtonTextAlign
    {
        get => Style.ButtonTextAlign;
    }

    public Vector2 SelectableTextAlign
    {
        get => Style.SelectableTextAlign;
    }

    public Vector2 DisplaySafeAreaPadding
    {
        get => Style.DisplaySafeAreaPadding;
    }

    public Color this[ImGuiCol col]
    {
        get => new(Style.Colors[(int)col]);
    }

    public Color Text
    {
        get => this[ImGuiCol.Text];
    }

    public Color TextDisabled
    {
        get => this[ImGuiCol.TextDisabled];
    }

    public Color WindowBg
    {
        get => this[ImGuiCol.WindowBg];
    }

    public Color ChildBg
    {
        get => this[ImGuiCol.ChildBg];
    }

    public Color PopupBg
    {
        get => this[ImGuiCol.PopupBg];
    }

    public Color Border
    {
        get => this[ImGuiCol.Border];
    }

    public Color FrameBg
    {
        get => this[ImGuiCol.FrameBg];
    }

    public Color FrameBgHovered
    {
        get => this[ImGuiCol.FrameBgHovered];
    }

    public Color FrameBgActive
    {
        get => this[ImGuiCol.FrameBgActive];
    }

    public Color TitleBg
    {
        get => this[ImGuiCol.TitleBg];
    }

    public Color TitleBgActive
    {
        get => this[ImGuiCol.TitleBgActive];
    }

    public Color TitleBgCollapsed
    {
        get => this[ImGuiCol.TitleBgCollapsed];
    }

    public Color MenuBarBg
    {
        get => this[ImGuiCol.MenuBarBg];
    }

    public Color ScrollbarBg
    {
        get => this[ImGuiCol.ScrollbarBg];
    }

    public Color ScrollbarGrab
    {
        get => this[ImGuiCol.ScrollbarGrab];
    }

    public Color ScrollbarGrabHovered
    {
        get => this[ImGuiCol.ScrollbarGrabHovered];
    }

    public Color ScrollbarGrabActive
    {
        get => this[ImGuiCol.ScrollbarGrabActive];
    }

    public Color CheckMark
    {
        get => this[ImGuiCol.CheckMark];
    }

    public Color SliderGrab
    {
        get => this[ImGuiCol.SliderGrab];
    }

    public Color SliderGrabActive
    {
        get => this[ImGuiCol.SliderGrabActive];
    }

    public Color Button
    {
        get => this[ImGuiCol.Button];
    }

    public Color ButtonHovered
    {
        get => this[ImGuiCol.ButtonHovered];
    }

    public Color ButtonActive
    {
        get => this[ImGuiCol.ButtonActive];
    }

    public Color Header
    {
        get => this[ImGuiCol.Header];
    }

    public Color HeaderHovered
    {
        get => this[ImGuiCol.HeaderHovered];
    }

    public Color HeaderActive
    {
        get => this[ImGuiCol.HeaderActive];
    }

    public Color Separator
    {
        get => this[ImGuiCol.Separator];
    }

    public Color SeparatorHovered
    {
        get => this[ImGuiCol.SeparatorHovered];
    }

    public Color SeparatorActive
    {
        get => this[ImGuiCol.SeparatorActive];
    }

    public Color ResizeGrip
    {
        get => this[ImGuiCol.ResizeGrip];
    }

    public Color ResizeGripHovered
    {
        get => this[ImGuiCol.ResizeGripHovered];
    }

    public Color ResizeGripActive
    {
        get => this[ImGuiCol.ResizeGripActive];
    }

    public Color Tab
    {
        get => this[ImGuiCol.Tab];
    }

    public Color TabHovered
    {
        get => this[ImGuiCol.TabHovered];
    }

    public Color TabActive
    {
        get => this[ImGuiCol.TabActive];
    }

    public Color TabUnfocused
    {
        get => this[ImGuiCol.TabUnfocused];
    }

    public Color TabUnfocusedActive
    {
        get => this[ImGuiCol.TabUnfocusedActive];
    }

    public Color DockingPreview
    {
        get => this[ImGuiCol.DockingPreview];
    }

    public Color DockingEmptyBg
    {
        get => this[ImGuiCol.DockingEmptyBg];
    }

    public Color TableHeaderBg
    {
        get => this[ImGuiCol.TableHeaderBg];
    }

    public Color TableBorderStrong
    {
        get => this[ImGuiCol.TableBorderStrong];
    }

    public Color TableBorderLight
    {
        get => this[ImGuiCol.TableBorderLight];
    }

    public Color TableRowBg
    {
        get => this[ImGuiCol.TableRowBg];
    }

    public Color TableRowBgAlt
    {
        get => this[ImGuiCol.TableRowBgAlt];
    }

    public Color TextSelectedBg
    {
        get => this[ImGuiCol.TextSelectedBg];
    }

    public Color DragDropTarget
    {
        get => this[ImGuiCol.DragDropTarget];
    }

    public Color NavHighlight
    {
        get => this[ImGuiCol.NavHighlight];
    }

    public Color NavWindowingHighlight
    {
        get => this[ImGuiCol.NavWindowingHighlight];
    }

    public Color NavWindowingDimBg
    {
        get => this[ImGuiCol.NavWindowingDimBg];
    }

    public Color ModalWindowDimBg
    {
        get => this[ImGuiCol.ModalWindowDimBg];
    }

    public Color DalamudRed { get; } = new(ImGuiColors.DalamudRed);

    public Color DalamudGrey { get; } = new(ImGuiColors.DalamudGrey);

    public Color DalamudGrey2 { get; } = new(ImGuiColors.DalamudGrey2);

    public Color DalamudGrey3 { get; } = new(ImGuiColors.DalamudGrey3);

    public Color DalamudWhite { get; } = new(ImGuiColors.DalamudWhite);

    public Color DalamudWhite2 { get; } = new(ImGuiColors.DalamudWhite2);

    public Color DalamudOrange { get; } = new(ImGuiColors.DalamudOrange);

    public Color TankBlue { get; } = new(ImGuiColors.TankBlue);

    public Color HealerGreen { get; } = new(ImGuiColors.HealerGreen);

    public Color DPSRed { get; } = new(ImGuiColors.DPSRed);

    public Color DalamudYellow { get; } = new(ImGuiColors.DalamudYellow);

    public Color DalamudViolet { get; } = new(ImGuiColors.DalamudViolet);

    public Color ParsedGrey { get; } = new(ImGuiColors.ParsedGrey);

    public Color ParsedGreen { get; } = new(ImGuiColors.ParsedGreen);

    public Color ParsedBlue { get; } = new(ImGuiColors.ParsedBlue);

    public Color ParsedPurple { get; } = new(ImGuiColors.ParsedPurple);

    public Color ParsedOrange { get; } = new(ImGuiColors.ParsedOrange);

    public Color ParsedPink { get; } = new(ImGuiColors.ParsedPink);
}
