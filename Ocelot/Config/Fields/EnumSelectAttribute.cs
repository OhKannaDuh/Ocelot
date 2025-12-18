using Ocelot.Config.Renderers;
using Ocelot.Config.Renderers.Enum;

namespace Ocelot.Config.Fields;

public class EnumSelectAttribute<TEnum, TDisplay, TFilter>()
    : UIFieldAttribute(typeof(EnumSelectRenderer<TEnum, TDisplay, TFilter>))
    where TEnum : struct, Enum
    where TDisplay : IEnumDisplay<TEnum>
    where TFilter : IEnumFilter<TEnum>;

public class EnumSelectAttribute<TEnum>
    : EnumSelectAttribute<TEnum, GenericDisplay<TEnum>, NoOpFilter<TEnum>>
    where TEnum : struct, Enum;

public class EnumSelectDisplayAttribute<TEnum, TDisplay>
    : EnumSelectAttribute<TEnum, TDisplay, NoOpFilter<TEnum>>
    where TEnum : struct, Enum
    where TDisplay : IEnumDisplay<TEnum>;

public class EnumSelectFilterAttribute<TEnum, TFilter>
    : EnumSelectAttribute<TEnum, GenericDisplay<TEnum>, TFilter>
    where TEnum : struct, Enum
    where TFilter : IEnumFilter<TEnum>;
