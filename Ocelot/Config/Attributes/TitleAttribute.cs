using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TitleAttribute : Attribute
{
    public string text { get; }

    public TitleAttribute(string text)
    {
        this.text = text;
    }
}
