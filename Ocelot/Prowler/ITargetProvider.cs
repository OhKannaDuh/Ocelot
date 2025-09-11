using System.Numerics;

namespace Ocelot.Prowler;

public interface ITargetProvider
{
    Vector3 GetCurrentPosition(ProwlContext context);
}
