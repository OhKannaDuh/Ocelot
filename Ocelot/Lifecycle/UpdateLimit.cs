using Ocelot.Services.Gate;

namespace Ocelot.Lifecycle;

public class UpdateLimit
{
    public UpdateLimitMode Mode { get; init; } = UpdateLimitMode.None;

    public long Limit { get; init; } = 0;

    public bool ShouldUpdate<T>(IGateService gates, T owner, string scope) where T : class
    {
        return Mode switch
        {
            UpdateLimitMode.None => true,
            UpdateLimitMode.UpdatesPerSecond => gates.UpdatesPerSecond(owner!, scope, (int)Limit),
            UpdateLimitMode.Milliseconds => gates.Milliseconds(owner!, scope, Limit),
            UpdateLimitMode.Seconds => gates.Seconds(owner!, scope, (int)Limit),
            UpdateLimitMode.Minutes => gates.Minutes(owner!, scope, (int)Limit),
            _ => true,
        };
    }

    public readonly static UpdateLimit None = new();

    public readonly static UpdateLimit SixtyTimesASecond = new()
    {
        Mode = UpdateLimitMode.UpdatesPerSecond,
        Limit = 60,
    };

    public readonly static UpdateLimit ThirtyTimesASecond = new()
    {
        Mode = UpdateLimitMode.UpdatesPerSecond, Limit = 30,
    };

    public readonly static UpdateLimit EverySecond = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 1,
    };

    public readonly static UpdateLimit EveryFiveSeconds = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 5,
    };

    public readonly static UpdateLimit EveryTenSeconds = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 10,
    };

    public readonly static UpdateLimit EveryFifteenSeconds = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 15,
    };

    public readonly static UpdateLimit EveryTwentySeconds = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 20,
    };

    public readonly static UpdateLimit EveryThirtySeconds = new()
    {
        Mode = UpdateLimitMode.Seconds, Limit = 30,
    };

    public readonly static UpdateLimit EveryMinute = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 1,
    };

    public readonly static UpdateLimit EveryFiveMinutes = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 5,
    };

    public readonly static UpdateLimit EveryTenMinutes = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 10,
    };

    public readonly static UpdateLimit EveryFifteenMinutes = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 15,
    };

    public readonly static UpdateLimit EveryTwentyMinutes = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 20,
    };

    public readonly static UpdateLimit EveryThirtyMinutes = new()
    {
        Mode = UpdateLimitMode.Minutes, Limit = 30,
    };
}
