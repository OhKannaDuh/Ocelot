using Ocelot.Modules;

namespace Ocelot.Data;

public class UpdateLimit
{
    public UpdateLimitMode Mode { get; init; } = UpdateLimitMode.None;

    public long Limit { get; init; } = 0;

    public bool ShouldUpdate<T>(T owner, UpdateContext context) where T : class
    {
        return Mode switch {
            UpdateLimitMode.None => true,
            UpdateLimitMode.UpdatesPerSecond => Gate.UpdatesPerSecond(owner, (int)Limit, context),
            UpdateLimitMode.Milliseconds => Gate.Milliseconds(owner, Limit, context),
            UpdateLimitMode.Seconds => Gate.Seconds(owner, (int)Limit, context),
            UpdateLimitMode.Minutes => Gate.Minutes(owner, (int)Limit, context),
            _ => true,
        };
    }

    public static readonly UpdateLimit None = new();

    public static readonly UpdateLimit SixtyTimesASecond = new() {
        Mode = UpdateLimitMode.UpdatesPerSecond,
        Limit = 60,
    };

    public static readonly UpdateLimit ThirtyTimesASecond = new() {
        Mode = UpdateLimitMode.UpdatesPerSecond,
        Limit = 30,
    };

    public static readonly UpdateLimit EverySecond = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 1,
    };

    public static readonly UpdateLimit EveryFiveSeconds = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 5,
    };

    public static readonly UpdateLimit EveryTenSeconds = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 10,
    };

    public static readonly UpdateLimit EveryFifteenSeconds = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 15,
    };

    public static readonly UpdateLimit EveryTwentySeconds = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 20,
    };

    public static readonly UpdateLimit EveryThirtySeconds = new() {
        Mode = UpdateLimitMode.Seconds,
        Limit = 30,
    };

    public static readonly UpdateLimit EveryMinute = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 1,
    };

    public static readonly UpdateLimit EveryFiveMinutes = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 5,
    };

    public static readonly UpdateLimit EveryTenMinutes = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 10,
    };

    public static readonly UpdateLimit EveryFifteenMinutes = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 15,
    };

    public static readonly UpdateLimit EveryTwentyMinutes = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 20,
    };

    public static readonly UpdateLimit EveryThirtyMinutes = new() {
        Mode = UpdateLimitMode.Minutes,
        Limit = 30,
    };
}
