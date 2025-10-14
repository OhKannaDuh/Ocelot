namespace Ocelot.Config.Fields;

[AttributeUsage(AttributeTargets.Property)]
public class UIFieldAttribute(Type rendererType) : Attribute
{
    public Type RendererType { get; } = rendererType ?? throw new ArgumentNullException(nameof(rendererType));

    public int Order { get; set; } = 0;
}
