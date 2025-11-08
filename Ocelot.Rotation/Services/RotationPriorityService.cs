using Ocelot.Rotation.Services.BossMod;
using Ocelot.Rotation.Services.RotationSolverReborn;
using Ocelot.Rotation.Services.Wrath;

namespace Ocelot.Rotation.Services;

public class RotationPriorityService : IRotationPriorityService
{
    public IEnumerable<string> GetPriority()
    {
        return
        [
            WrathRotationProvider.Key,
            BossModRotationProvider.Key,
            RotationSolverRebornRotationProvider.Key,
        ];
    }
}
