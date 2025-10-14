using Dalamud.Interface;
using Ocelot.Graphics;

namespace Ocelot.UI.ComposableStrings;

public class FontIcon(FontAwesomeIcon icon, Color color) : Text(icon.ToIconString(), color, UiBuilder.IconFont);
