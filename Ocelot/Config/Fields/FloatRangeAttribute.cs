using Ocelot.Config.Renderers;

namespace Ocelot.Config.Fields;

public sealed class FloatRangeAttribute(float min, float max) : UIFieldAttribute(typeof(FloatRangeRenderer))
{
    public float Min { get; } = min;

    public float Max { get; } = max;
}
