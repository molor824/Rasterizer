using System.Numerics;

public static class ExtensionMethods
{
    public static float Lerp(this float a, float b, float v)
    {
        return (b - a) * v + a;
    }
    public static float InvLerp(this float a, float b, float v)
    {
        return (v - a) / (b - a);
    }
    public static Color Lerp(this Color a, Color b, float v)
    {
        return new(
            (byte)MathF.Round(((float)a.r).Lerp(b.r, v)),
            (byte)MathF.Round(((float)a.g).Lerp(b.g, v)),
            (byte)MathF.Round(((float)a.b).Lerp(b.b, v)),
            (byte)MathF.Round(((float)a.a).Lerp(b.a, v))
        );
    }
    public static vec3 Lerp(this vec3 a, vec3 b, float v)
    {
        return new(
            a.x.Lerp(b.x, v),
            a.y.Lerp(b.y, v),
            a.z.Lerp(b.z, v)
        );
    }
    public static Vector2 ToNumerics(this vec2 a)
    {
        return new(a.x, a.y);
    }
    public static Vector3 ToNumerics(this vec3 a)
    {
        return new(a.x, a.y, a.z);
    }
}