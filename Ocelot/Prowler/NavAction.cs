using System.Numerics;
using ECommons.Automation.NeoTaskManager;

namespace Ocelot.Prowler;

public abstract class NavAction : IProwlerAction
{
    private Vector3 destination;

    public NavAction(Vector3 destination)
    {
        this.destination = destination;
    }

    protected abstract void Init(ProwlerContext context);

    protected abstract string GetKey();

    public string Identify() => GetKey();

    public TaskManagerTask Create(ProwlerContext context)
    {
        return new(() =>
        {
            if (context.ShouldInit())
            {
                Init(context);
                return false;
            }

            if (!context.HasStarted() && context.vnav.IsRunning())
            {
                context.Start();
                return false;
            }

            return context.Check(GetKey(), destination);
        });
    }
}
