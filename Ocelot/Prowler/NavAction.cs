using System.Numerics;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

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
                Logger.Debug($"[Prowloer] Initialising Prowler Action: {GetKey()}");
                Init(context);
                return false;
            }


            if (!context.HasStarted())
            {
                if (context.vnav.IsRunning())
                {
                    Logger.Debug($"[Prowler] Starting: {GetKey()}");
                    context.Start();
                }

                return false;
            }

            var isAtDestination = context.IsAtDestination(destination);
            if (!context.vnav.IsRunning() && !isAtDestination)
            {
                throw new System.Exception("Vnav stopped before reaching destination");
            }

            return isAtDestination;
        }, new() { TimeLimitMS = int.MaxValue });
    }
}
