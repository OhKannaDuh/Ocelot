using System;

namespace Ocelot.Config.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TitleAttribute(string translationKey = "config.title") : Attribute
{
    public string translation_key { get; } = translationKey;
}
