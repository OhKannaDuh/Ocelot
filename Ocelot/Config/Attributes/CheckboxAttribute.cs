using System;
using Ocelot.Config.Handlers;
using Ocelot.Modules;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class CheckboxAttribute : ConfigAttribute
{
    public override Handler GetHandler(ModuleConfig self) => new Checkbox(self, this);
}
