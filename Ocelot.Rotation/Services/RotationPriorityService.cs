using Ocelot.Rotation.Services.BossMod;

namespace Ocelot.Rotation.Services;

public class RotationPriorityService : IRotationPriorityService
{
    public IEnumerable<string> GetPriority()
    {
        return
        [
            BossModRotationProvider.Key,
        ];
    }
}
