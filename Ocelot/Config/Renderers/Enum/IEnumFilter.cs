namespace Ocelot.Config.Renderers.Enum;

public interface IEnumFilter<in TEnum>
    where TEnum : struct, System.Enum
{
    bool Filter(TEnum value);
}
