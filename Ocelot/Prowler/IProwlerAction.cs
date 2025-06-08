using ECommons.Automation.NeoTaskManager;

namespace Ocelot.Prowler;

public interface IProwlerAction
{
    TaskManagerTask Create(ProwlerContext context);

    string Identify();
}