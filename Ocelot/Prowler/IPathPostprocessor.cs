namespace Ocelot.Prowler;

public interface IPathPostprocessor
{
    void Postprocess(ProwlContext context);
}
