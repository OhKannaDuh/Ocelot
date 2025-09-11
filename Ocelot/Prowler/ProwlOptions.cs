using Ocelot.Services;

namespace Ocelot.Prowler;

public sealed class ProwlOptions
{
    public IMovementPolicy Movement { get; init; } = OcelotServices.GetCached<IMovementPolicy>();

    public IPathPreprocessor Preprocessor { get; init; } = OcelotServices.GetCached<IPathPreprocessor>();

    public IPathPostprocessor Postprocessor { get; init; } = OcelotServices.GetCached<IPathPostprocessor>();

    public IProwlWatcher Watcher { get; init; } = OcelotServices.GetCached<IProwlWatcher>();

    public ITargetProvider? TargetProvider { get; init; } = OcelotServices.GetOptional<ITargetProvider>();

    public IPathInterceptor? Interceptor { get; init; } = OcelotServices.GetOptional<IPathInterceptor>();

    public IEventHandler? EventHandler { get; init; } = OcelotServices.GetOptional<IEventHandler>();

    public uint MountId { get; init; } = 0;

    public float RetargetTolerance { get; init; } = 1f;

    public float ArrivalRadius { get; init; } = 0.4f;
}
