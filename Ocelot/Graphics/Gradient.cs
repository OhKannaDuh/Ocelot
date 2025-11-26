namespace Ocelot.Graphics;

public class Gradient(List<Gradient.Entry>? entries = null)
{
    public record struct Entry(float Position, Color Color);

    private readonly List<Entry> entries = entries ?? [];

    public void AddEntry(float position, Color color)
    {
        entries.Add(new Entry(position, color));
        entries.Sort((a, b) => a.Position.CompareTo(b.Position));
    }

    public Color Sample(float t)
    {
        if (entries.Count == 0)
        {
            return Color.Black;
        }

        if (entries.Count == 1)
        {
            return entries[0].Color;
        }

        if (t <= entries[0].Position)
        {
            return entries[0].Color;
        }

        if (t >= entries[^1].Position)
        {
            return entries[^1].Color;
        }

        for (var i = 0; i < entries.Count - 1; i++)
        {
            var a = entries[i];
            var b = entries[i + 1];

            if (t >= a.Position && t <= b.Position)
            {
                var normalized = (t - a.Position) / (b.Position - a.Position);
                return Lerp(a.Color, b.Color, normalized);
            }
        }

        return entries[^1].Color;
    }

    private static Color Lerp(Color a, Color b, float t)
    {
        var av = a.ToVector4();
        var bv = b.ToVector4();

        return new Color(
            av.X + (bv.X - av.X) * t,
            av.Y + (bv.Y - av.Y) * t,
            av.Z + (bv.Z - av.Z) * t,
            av.W + (bv.W - av.W) * t
        );
    }
}
