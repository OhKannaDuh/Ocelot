namespace Ocelot.Config;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
public sealed class ConfigHiddenAttribute : Attribute;
