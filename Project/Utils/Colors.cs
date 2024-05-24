using System.Drawing;
using OpenTK;
using OpenTK.Mathematics;

namespace rt004.Utils;

public static class Colors
{
    public static Color3<TColor> Add<TColor>(this Color3<TColor> color1, Color3<TColor> color2)
        where TColor : IColorSpace3
    {
        return new Color3<TColor>(
            Clamp(color1.X + color2.X),
            Clamp(color1.Y + color2.Y),
            Clamp(color1.Z + color2.Z));
    }
    
    public static Color3<TColor> Multiply<TColor>(this Color3<TColor> color, float coefficient)
        where TColor : IColorSpace3
    {
        return new Color3<TColor>(
            Clamp(color.X * coefficient),
            Clamp(color.Y * coefficient),
            Clamp(color.Z * coefficient));
    }

    public static Color3<TColor> Multiply<TColor>(this Color3<TColor> color1, Color3<TColor> color2)
        where TColor : IColorSpace3
    {
        return new Color3<TColor>(
            Clamp(color1.X * color2.X),
            Clamp(color1.Y * color2.Y),
            Clamp(color1.Z * color2.Z));
    }

    public static Color3<TColor> Average<TColor>(List<Color3<TColor>> colors)
        where TColor : IColorSpace3
    {
        float xSum = 0;
        float ySum = 0;
        float zSum = 0;
        
        foreach (var color in colors)
        {
            xSum += color.X;
            ySum += color.Y;
            zSum += color.Z;
        }

        return new Color3<TColor>(
            xSum / colors.Count,
            ySum / colors.Count,
            zSum / colors.Count);
    }
    
    public static Vector3 ToVector<TColor>(this Color3<TColor> color)
        where TColor : IColorSpace3
    {
        return new Vector3(color.X, color.Y, color.Z);
    }

    public static Color3<Rgb> FromVector(Vector3 vector)
    {
        return new Color3<Rgb>(vector.X, vector.Y, vector.Z);
    }

    private static float Clamp(float value, float lowerBound = 0, float upperBound = 1)
    {
        if (value > upperBound) return upperBound;
        if (value < lowerBound) return lowerBound;
        return value;
    }
}