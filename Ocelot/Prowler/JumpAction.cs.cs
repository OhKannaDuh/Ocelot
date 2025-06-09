using ECommons.Automation.NeoTaskManager;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Ocelot.Prowler;

public class JumpAction : IProwlerAction
{
    public unsafe TaskManagerTask Create(ProwlerContext context)
    {
        return new(() =>
        {
            if (context.ShouldInit())
            {
                context.Jump();
                return false;
            }

            return !Player.IsJumping;
        });
    }

    public string Identify() => "Prowler.JumpAction";
}
