namespace Ocelot.Rotation.Services;

public interface IRotationPriorityService
{
    IEnumerable<string> GetPriority();
}
