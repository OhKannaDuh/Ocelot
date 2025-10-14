namespace Ocelot.Config;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConfigGroupAttribute(string key) : Attribute
{
    public string Key { get; } = key;

    public int Order { get; init; } = 0;
}
