using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class LabelAttribute : Attribute
{
    public string translation_key { get; }

    public LabelAttribute(string translation_key)
    {
        this.translation_key = translation_key;
    }
}
