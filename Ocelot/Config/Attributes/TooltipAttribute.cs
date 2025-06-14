using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
    public string translation_key { get; }

    public TooltipAttribute(string translation_key)
    {
        this.translation_key = translation_key;
    }
}
