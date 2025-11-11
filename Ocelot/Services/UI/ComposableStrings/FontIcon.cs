using Dalamud.Interface;
using Ocelot.Graphics;

namespace Ocelot.Services.UI.ComposableStrings;

public class FontIcon(FontAwesomeIcon icon, Color color) : Text(icon.ToIconString(), color, UiBuilder.IconFont);
