namespace Ocelot.Prowler;

public enum ProwlState
{
    NotStarted,

    Preprocessing,

    Pathfinding,

    Postprocessing,

    PreparingMovement, // Here we determine if we should mount or go straight to movement

    Mount,

    Moving,

    Redirecting,

    Cancelled,

    Complete,

    Faulted,
}
