// using System.Numerics;
// using Dalamud.Bindings.ImGui;
// using Dalamud.Interface.Utility.Raii;
//
// namespace Ocelot.UI.ComposableStrings;
//
// public class Button(string id) : IComposable
// {
//     private string id = id;
//
//     public ComposableGroup Content { get; private set; } = new();
//
//     public Vector2 Padding { get; private set; } = new(8, 4);
//
//     public float MinWidth { get; private set; } = 0f;
//
//     public bool Disabled { get; private set; } = false;
//
//     public string? Tooltip { get; private set; }
//
//     public Action? OnClick { get; private set; }
//
//     public Action? OnRightClick { get; private set; }
//
//     public Button(ComposableGroup content, string id)
//         : this(id)
//     {
//         Content = content;
//     }
//
//     public Button WithContent(ComposableGroup group)
//     {
//         Content = group;
//         return this;
//     }
//
//     public Button WithPadding(Vector2 px)
//     {
//         Padding = px;
//         return this;
//     }
//
//     public Button WithMinWidth(float w)
//     {
//         MinWidth = w;
//         return this;
//     }
//
//     public Button SetDisabled(bool disabled = true)
//     {
//         Disabled = disabled;
//         return this;
//     }
//
//     public Button WithTooltip(string? tooltip)
//     {
//         Tooltip = tooltip;
//         return this;
//     }
//
//     public Button OnLeftClick(Action? callback)
//     {
//         OnClick = callback;
//         return this;
//     }
//
//     public Button OnRClick(Action? callback)
//     {
//         OnRightClick = callback;
//         return this;
//     }
//
//     public float GetWidth()
//     {
//         var inner = Content.Width;
//         var width = inner + Padding.X * 2f;
//         return MinWidth > 0 ? MathF.Max(width, MinWidth) : width;
//     }
//
//     public float GetHeight()
//     {
//         return ImGui.GetFrameHeight();
//     }
//
//     public bool Render()
//     {
//         var style = ImGui.GetStyle();
//         var draw = ImGui.GetWindowDrawList();
//
//         var size = new Vector2(GetWidth(), 0f);
//
//         using var group = ImRaii.Group();
//
//         ImGui.Button($"##btn_{id}", size);
//
//         var hovered = ImGui.IsItemHovered();
//         var active = ImGui.IsItemActive();
//
//         var min = ImGui.GetItemRectMin();
//         var max = ImGui.GetItemRectMax();
//
//         var col =
//             Disabled ? ImGui.GetColorU32(ImGuiCol.Button) :
//             active ? ImGui.GetColorU32(ImGuiCol.ButtonActive) :
//             hovered ? ImGui.GetColorU32(ImGuiCol.ButtonHovered) :
//             ImGui.GetColorU32(ImGuiCol.Button);
//
//         draw.AddRectFilled(min, max, col, style.FrameRounding);
//         draw.AddRect(min, max, ImGui.GetColorU32(ImGuiCol.Border), style.FrameRounding);
//
//         var contentPos = new Vector2(
//             min.X + Padding.X,
//             min.Y + (max.Y - min.Y - Content.) / 2
//         );
//
//         ImGui.SetCursorScreenPos(contentPos);
//         Content.Render();
//
//         if (hovered && !string.IsNullOrEmpty(Tooltip))
//         {
//             ImGui.SetTooltip(Tooltip!);
//         }
//
//         if (Disabled)
//         {
//             return hovered;
//         }
//
//         if (hovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
//         {
//             OnClick?.Invoke();
//         }
//
//         if (hovered && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
//         {
//             OnRightClick?.Invoke();
//         }
//
//         return hovered;
//     }
// }



