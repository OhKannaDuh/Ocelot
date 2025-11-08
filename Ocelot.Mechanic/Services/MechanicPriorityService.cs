using Ocelot.Mechanic.Services.BossMod;
using Ocelot.Mechanic.Services.BossModReborn;

namespace Ocelot.Mechanic.Services;

public class MechanicPriorityService : IMechanicPriorityService
{
    public IEnumerable<string> GetPriority()
    {
        return
        [
            BossModMechanicProvider.Key,
            BossModRebornMechanicProvider.Key,
        ];
    }
}
