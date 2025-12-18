namespace Ocelot.Config.Renderers.Enum;

public interface IEnumDisplay<in TEnum>
    where TEnum : struct, System.Enum
{
    string Display(TEnum value);
}
