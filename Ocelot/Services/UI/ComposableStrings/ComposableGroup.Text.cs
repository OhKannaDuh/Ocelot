using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Ocelot.Graphics;

namespace Ocelot.Services.UI.ComposableStrings;

public partial class ComposableGroup
{
    public ComposableGroup Text(Text text)
    {
        composables.Add(text);
        return this;
    }

    public ComposableGroup Text(string text)
    {
        return Text(new Text(text, branding.Text, UiBuilder.DefaultFont));
    }

    public ComposableGroup Text(string text, Color color)
    {
        return Text(new Text(text, color, UiBuilder.DefaultFont));
    }

    public ComposableGroup Text(string text, ImFontPtr font)
    {
        return Text(new Text(text, branding.Text, font));
    }
}
