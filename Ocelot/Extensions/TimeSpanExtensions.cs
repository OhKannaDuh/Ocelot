namespace Ocelot.Extensions;

public static class TimeSpanExtensions
{
    extension(TimeSpan self)
    {
        public string Format()
        {
            if (self.TotalHours > 0)
            {
                return $"{(int)self.TotalHours:00}:{self.Minutes:00}:{self.Seconds:00}";
            }

            return $"{self.Minutes:00}:{self.Seconds:00}";
        }
    }
}
