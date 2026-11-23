using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;

namespace Ocelot.Services.Gate;

public sealed class GateService(IFramework framework) : IGateService
{
    private sealed class ScopeBucket
    {
        public readonly Dictionary<string, long> Elapsed = new();
    }

    private readonly ConditionalWeakTable<object, ScopeBucket> buckets = new();

    public bool Milliseconds(object owner, string scope, long interval)
    {
        if (interval <= 0)
        {
            return true;
        }

        var bucket = buckets.GetOrCreateValue(owner);

        bucket.Elapsed.TryGetValue(scope, out var elapsed);
        bucket.Elapsed[scope] = Advance(elapsed, framework.UpdateDelta, interval, out var due);
        return due;
    }

    // Ticks, not whole milliseconds: at 144 fps cutting each frame to 6 ms made a 500 ms limit fire every ~580 ms.
    public static long Advance(long elapsedTicks, TimeSpan delta, long intervalMs, out bool due)
    {
        long intervalTicks = intervalMs * TimeSpan.TicksPerMillisecond;
        elapsedTicks += delta.Ticks;
        due = elapsedTicks >= intervalTicks;
        return due ? elapsedTicks - intervalTicks : elapsedTicks;
    }

    public bool UpdatesPerSecond(object owner, string scope, int ups)
    {
        return Milliseconds(owner, scope, ups <= 0 ? 0 : 1000 / ups);
    }

    public bool Seconds(object owner, string scope, int seconds)
    {
        return Milliseconds(owner, scope, (long)seconds * 1000);
    }

    public bool Minutes(object owner, string scope, int minutes)
    {
        return Seconds(owner, scope, minutes * 60
        );
    }

    public void Reset(object owner)
    {
        if (buckets.TryGetValue(owner, out var bucket))
        {
            bucket.Elapsed.Clear();
        }
    }

    public void Reset(object owner, string scope)
    {
        if (buckets.TryGetValue(owner, out var bucket))
        {
            bucket.Elapsed.Remove(scope);
        }
    }
}
