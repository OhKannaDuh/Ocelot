using ECommons.Automation;

namespace Ocelot.Mechanic.Services.BossMod;

public class BossModMechanicService : IMechanicService
{
    public void Enable()
    {
        Chat.ExecuteCommand("/vbmai on");
    }

    public void Disable()
    {
        Chat.ExecuteCommand("/vbmai off");
    }
}
