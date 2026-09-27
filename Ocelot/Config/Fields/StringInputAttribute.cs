using Ocelot.Config.Renderers;

namespace Ocelot.Config.Fields;

public class StringInputAttribute() : UIFieldAttribute(typeof(StringInputRenderer))
{
    public int MaxLength { get; set; } = 256;
}
