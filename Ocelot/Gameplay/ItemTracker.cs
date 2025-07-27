using System;

namespace Ocelot.Gameplay;

public class ItemTracker(Item item)
{
    private DateTime start = DateTime.UtcNow;

    private int last = item.Count();

    private int gained = 0;

    public void Reset()
    {
        start = DateTime.UtcNow;
        last = item.Count();
        gained = 0;
    }

    public void Update()
    {
        var current = item.Count();
        var delta = current - last;
        last = current;

        if (delta > 0)
        {
            if (gained == 0)
            {
                start = DateTime.UtcNow;
            }

            gained += delta;
        }
    }

    public float GetGainPerHour()
    {
        var elapsed = (float)(DateTime.UtcNow - start).TotalHours;
        if (elapsed <= 0)
        {
            return 0;
        }

        return MathF.Round(gained / elapsed, 2);
    }
}
