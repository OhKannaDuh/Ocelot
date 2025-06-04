using System;
using Ocelot.Config.Handlers;
using Ocelot.Modules;

namespace Ocelot.Config.Attributes;

public abstract partial class ConfigAttribute : Attribute
{
    public abstract Handler GetHandler(ModuleConfig self);
}
