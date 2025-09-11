namespace Ocelot.Services.Pathfinding;

[OcelotService(typeof(IPathfinderServiceProvider))]
public class DefaultPathfinderServiceProvider : IPathfinderServiceProvider
{
    public IPathfinderService GetService()
    {
        if (ProwlerPathfinderService.RequirementsMet())
        {
            return OcelotServices.Container.Get<ProwlerPathfinderService>();
        }

        if (NavmeshPathfinderService.RequirementsMet())
        {
            return OcelotServices.Container.Get<NavmeshPathfinderService>();
        }

        return OcelotServices.Container.Get<BlankPathfinderService>();
    }
}
