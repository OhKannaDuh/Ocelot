using System;
using Ocelot.Config.Handlers;
using Ocelot.Modules;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelSheetAttribute : ConfigAttribute
{
    private Type type { get; }

    private readonly string provider;

    public ExcelSheetAttribute(Type type, string provider)
    {
        this.type = type;
        this.provider = provider;
    }

    public override Handler GetHandler(ModuleConfig self) =>
        (Handler)Activator.CreateInstance(typeof(ExcelSheet<>).MakeGenericType(type), self, this, provider)!;
}
