using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TitleAttribute : Attribute
{
    public string translation_key { get; }

    public TitleAttribute(string translation_key)
    {
        this.translation_key = translation_key;
    }
}
