using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
    public string text { get; }

    public TooltipAttribute(string text)
    {
        this.text = text;
    }
}
