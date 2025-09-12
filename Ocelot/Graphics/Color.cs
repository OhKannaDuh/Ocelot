using System.Numerics;

namespace Ocelot.Graphics;

public readonly struct Color
{
    public byte R { get; }

    public byte G { get; }

    public byte B { get; }

    public byte A { get; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(float r, float g, float b, float a = 1f)
    {
        R = (byte)(Clamp01(r) * 255);
        G = (byte)(Clamp01(g) * 255);
        B = (byte)(Clamp01(b) * 255);
        A = (byte)(Clamp01(a) * 255);
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

    public override string ToString()
    {
        return $"Color(R:{R}, G:{G}, B:{B}, A:{A})";
    }

    public static Color White {
        get => new(255, 255, 255, 255);
    }

    public static Color Black {
        get => new(0, 0, 0, 255);
    }

    public static Color Transparent {
        get => new(0, 0, 0, 0);
    }

    public static Color Red {
        get => new(255, 0, 0, 255);
    }

    public static Color Green {
        get => new(0, 255, 0, 255);
    }

    public static Color Blue {
        get => new(0, 0, 255, 255);
    }
}
