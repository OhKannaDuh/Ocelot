namespace Ocelot.Prowler;

public interface IPathInterceptor
{
    // Return try if we've modified the path
    bool TryIntercept(ProwlContext context);
}
