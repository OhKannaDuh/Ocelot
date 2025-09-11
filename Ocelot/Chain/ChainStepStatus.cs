namespace Ocelot.Chain;

public enum ChainStepStatus
{
    Continue,

    Requeue,

    InsertSteps,

    Done,

    Abort,

    Fail,
}
