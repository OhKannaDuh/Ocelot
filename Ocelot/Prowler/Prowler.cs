using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ocelot.Prowler;

public class Prowler
{
    private readonly ProwlerContext context;

    public Prowler(ProwlerContext context)
    {
        this.context = context;
    }

    public async Task Run(IEnumerable<IProwlerAction> actions)
    {
        foreach (var action in actions)
        {
            await action.ExecuteAsync(context);
        }
    }
}
