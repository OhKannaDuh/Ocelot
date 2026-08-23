namespace Ocelot.Config.Fields;

[AttributeUsage(AttributeTargets.Property)]
public class UIFieldAttribute(Type rendererType) : Attribute
{
    public Type RendererType { get; } = rendererType ?? throw new ArgumentNullException(nameof(rendererType));

    public int Order { get; set; } = 0;

    /// <summary>Nest this field under a parent control (each level ≈ 16px).</summary>
    public int Indent { get; set; } = 0;

    /// <summary>
    ///     Name of a bool property on the same config object. When that property is false,
    ///     this field is drawn disabled (still visible as a child of the parent toggle).
    /// </summary>
    public string? Requires { get; set; }

    /// <summary>
    ///     Name of a bool property on the same config object. When that property is true,
    ///     this field is drawn disabled (e.g. an alternative that another option replaces).
    /// </summary>
    public string? DisabledWhen { get; set; }

    /// <summary>
    ///     Optional section key within this config page. When it changes between fields,
    ///     ConfigRenderer draws a small titled divider. Resolved as
    ///     <c>config.{page}.sections.{key}</c>.
    /// </summary>
    public string? Section { get; set; }
}
