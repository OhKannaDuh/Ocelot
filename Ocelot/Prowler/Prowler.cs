using Dalamud.Plugin.Services;
using Ocelot.Ipc;
using Ocelot.Services;
using Ocelot.Services.Pathfinding;

namespace Ocelot.Prowler;

public static class Prowler
{
    private static IPathfinderService Pathfinder {
        get => OcelotServices.GetCached<IPathfinderService>();
    }

    public static bool IsRunning {
        get => Current != null;
    }

    private static ProwlerStateMachine? Current;

    public static ProwlState? State {
        get => Current?.State;
    }

    public static void Prowl(Prowl prowl)
    {
        if (IsRunning)
        {
            throw new AlreadyProwlingException();
        }

        Current = new ProwlerStateMachine(prowl);
    }

    public static void Update(IFramework _)
    {
        if (Current == null)
        {
            return;
        }

        Current.Update();

        var prowl = Current.GetProwl();
        var context = prowl.Context;

        switch (Current.State)
        {
            case ProwlState.Faulted:
                prowl.Options.EventHandler?.OnFaulted(context);
                prowl.Options.EventHandler?.Finally(context);
                Current.Dispose();
                Current = null;
                break;
            case ProwlState.Cancelled:
                prowl.Options.EventHandler?.OnCancelled(context);
                prowl.Options.EventHandler?.Finally(context);
                Current.Dispose();
                Current = null;
                break;
            case ProwlState.Complete:
                prowl.Options.EventHandler?.OnComplete(context);
                prowl.Options.EventHandler?.Finally(context);
                Current.Dispose();
                Current = null;
                break;
        }
    }

    public static void Abort()
    {
        Current?.Dispose();
        Current = null;
        VNavmesh.Stop();
    }
}
