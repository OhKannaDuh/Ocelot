using ECommons.Automation.NeoTaskManager;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Prowler;

public class JumpAction : IProwlerAction
{
    public unsafe TaskManagerTask Create(ProwlerContext context)
    {
        return new(() =>
        {
            context.Jump();
        });
    }

    public string Identify() => "Prowler.JumpAction";
}
