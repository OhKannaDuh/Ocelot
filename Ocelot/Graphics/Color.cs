using System.Numerics;

namespace Ocelot.Graphics;

public readonly struct Color(byte r, byte g, byte b, byte a = 255)
{
    private byte R { get; } = r;

    private byte G { get; } = g;

    private byte B { get; } = b;

    private byte A { get; } = a;

    public Color(float r, float g, float b, float a = 1f) : this((byte)(Clamp01(r) * 255), (byte)(Clamp01(g) * 255), (byte)(Clamp01(b) * 255),
        (byte)(Clamp01(a) * 255))
    {
    }

    public Color(Vector4 vec) : this(vec.X, vec.Y, vec.Z, vec.W)
    {
    }

    private static float Clamp01(float v)
    {
        return v < 0 ? 0 : v > 1 ? 1 : v;
    }

    public uint ToRgba()
    {
        return (uint)(R | G << 8 | B << 16 | A << 24);
    }

    public static Color FromRgba(uint rgba)
    {
        var r = (byte)(rgba & 0xFF);
        var g = (byte)(rgba >> 8 & 0xFF);
        var b = (byte)(rgba >> 16 & 0xFF);
        var a = (byte)(rgba >> 24 & 0xFF);

        return new Color(r, g, b, a);
    }

    public string ToHex(bool includeAlpha = false)
    {
        return includeAlpha
            ? $"#{R:X2}{G:X2}{B:X2}{A:X2}"
            : $"#{R:X2}{G:X2}{B:X2}";
    }

    public Vector4 ToVector4()
    {
        const float inv = 1f / 255f;
        return new Vector4(
            R * inv,
            G * inv,
            B * inv,
            A * inv
        );
    }

    public Color WithAlpha(float alpha)
    {
        return new Color(R, G, B, (byte)(Clamp01(alpha) * 255));
    }

    public override string ToString()
    {
        return $"Color(R:{R}, G:{G}, B:{B}, A:{A})";
    }

    public static Color White
    {
        get => new(255, 255, 255, 255);
    }

    public static Color Black
    {
        get => new(0, 0, 0, 255);
    }

    public static Color Transparent
    {
        get => new(0, 0, 0, 0);
    }

    public static Color Red
    {
        get => new(255, 0, 0, 255);
    }

    public static Color Green
    {
        get => new(0, 255, 0, 255);
    }

    public static Color Blue
    {
        get => new(0, 0, 255, 255);
    }
}
