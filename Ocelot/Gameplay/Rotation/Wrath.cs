using System;
using ECommons.DalamudServices;
using Ocelot.IPC;

namespace Ocelot.Gameplay.Rotation;

public class Wrath : BaseRotationPlugin
{
    public override string DisplayName
    {
        get => "Wrath Combo";
    }

    public override string InternalName
    {
        get => "WrathCombo";
    }

    public override string Author
    {
        get => "Team Wrath";
    }

    private readonly WrathCombo wrath;

    private readonly Guid leaseId;

    public Wrath(OcelotPlugin plugin)
    {
        wrath = plugin.IPC.GetSubscriber<WrathCombo>();
        var lease = wrath.RegisterForLease(Svc.PluginInterface.InternalName, plugin.Name);
        if (lease == null)
        {
            throw new Exception("Unable to create Wrath Combo");
        }

        leaseId = (Guid)lease;
    }

    public override void Enable()
    {
    }

    public override void Disable()
    {
    }

    public override void Dispose()
    {
        wrath.ReleaseControl(leaseId);
    }
}
