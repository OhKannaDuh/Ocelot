using Dalamud.Interface;
using Ocelot.Graphics;

namespace Ocelot.UI.ComposableStrings;

public partial class ComposableGroup
{
    public ComposableGroup Icon(FontIcon icon)
    {
        composables.Add(icon);
        return this;
    }

    public ComposableGroup Icon(FontAwesomeIcon icon)
    {
        return Icon(new FontIcon(icon, branding.Text));
    }

    public ComposableGroup Icon(FontAwesomeIcon icon, Color color)
    {
        return Icon(new FontIcon(icon, color));
    }
}
