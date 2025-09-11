namespace Ocelot.Prowler;

public interface IMovementPolicy
{
    bool ShouldFly(in ProwlContext context);

    bool ShouldMount(in ProwlContext context);

    bool ShouldSprint(in ProwlContext context);
}
