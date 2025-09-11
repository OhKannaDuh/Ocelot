using System.Numerics;

namespace Ocelot.Prowler;

public class StaticTargetProvider(Vector3 destination) : ITargetProvider
{
    public Vector3 GetCurrentPosition(ProwlContext context)
    {
        return destination;
    }
}
