using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace Ocelot.Windows;

public class DynamicTitleBarButton : Window.TitleBarButton
{
    private Action<DynamicTitleBarButton, ImGuiMouseButton> OnClick;

    public DynamicTitleBarButton(Action<DynamicTitleBarButton, ImGuiMouseButton> OnClick)
    {
        this.OnClick = OnClick;
        Click = m => OnClick(this, m);
    }
}
