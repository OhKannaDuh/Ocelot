using System.Numerics;
using System.Threading.Tasks;

namespace Ocelot.Prowler;

public class MoveAction : IProwlerAction
{
    public Vector3 destination { get; }

    public MoveAction(Vector3 destination) => this.destination = destination;

    public async Task ExecuteAsync(ProwlerContext context)
    {
        // await context.IPC.SendAsync(new MoveCommand(Destination));
        // await context.WaitForMovementCompleteAsync();
    }
}
