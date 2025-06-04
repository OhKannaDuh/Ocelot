using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class LabelAttribute : Attribute
{
    public string text { get; }

    public LabelAttribute(string text)
    {
        this.text = text;
    }
}
