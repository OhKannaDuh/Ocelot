namespace Ocelot.Config.Renderers.Enum;

public class GenericDisplay<TEnum> : IEnumDisplay<TEnum>
    where TEnum : struct, System.Enum
{
    public string Display(TEnum value)
    {
        return value.ToString();
    }
}
