using ECommons.Automation;

namespace Ocelot.Mechanic.Services.BossModReborn;

public class BossModRebornMechanicService : IMechanicService
{
    public void Enable()
    {
        Chat.ExecuteCommand("/bmrai on");
    }

    public void Disable()
    {
        Chat.ExecuteCommand("/bmrai off");
    }
}
