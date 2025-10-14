using Ocelot.Config.Renderers;

namespace Ocelot.Config.Fields;

public sealed class IntRangeAttribute(int min, int max) : UIFieldAttribute(typeof(IntRangeRenderer))
{
    public int Min { get; } = min;

    public int Max { get; } = max;
}
