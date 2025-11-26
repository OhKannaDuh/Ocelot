namespace Ocelot.Config.Renderers.Enum;

public class NoOpFilter<TEnum> : IEnumFilter<TEnum>
    where TEnum : struct, System.Enum
{
    public bool Filter(TEnum value)
    {
        return true;
    }
}
