namespace rt004.Utils;

using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

public static class VectorHelper
{
    public static Vector3 To3d(this Vector2 vector, float z = 0)
    {
        return new Vector3(vector.X, vector.Y, z);
    }
    
    public static Vector3 To3d(this Vector4 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
    
    public static Vector4 ToHomogenous(this Vector3 vector, float w = 1)
    {
        return new Vector4(vector.X, vector.Y, vector.Z, w);
    }
    
    public static Vector4 ToHomogenous(this Vector2 vector, float z = 0, float w = 1)
    {
        return new Vector4(vector.X, vector.Y, z, w);
    }
    
    public static IEnumerable<Vector2> IterateRectangle(Vector2 topLeft, Vector2 bottomRight)
    {
        for (int x = topLeft.XInt(); x < bottomRight.XInt(); x++)
        {
            for (int y = topLeft.YInt(); y < bottomRight.YInt(); y++)
            {
                yield return new Vector2(x, y);
            }
        }
    }
    
    public static IEnumerable<Vector2> IterateRectangle(int width, int height)
    {
        return IterateRectangle(new Vector2(0, 0), new Vector2(width, height));
    }
    
    public static IEnumerable<Vector3> IterateCube(Vector3 topLeft, Vector3 bottomRight)
    {
        for (int x = topLeft.XInt(); x < bottomRight.XInt(); x++)
        {
            for (int y = topLeft.YInt(); y < bottomRight.YInt(); y++)
            {
                for (int z = topLeft.ZInt(); z < bottomRight.ZInt(); z++)
                {
                    yield return new Vector3(x, y, z);
                }
            }
        }
    }
    
    public static IEnumerable<Vector3> IterateCube(int width, int height, int depth)
    {
        return IterateCube(new Vector3(0, 0, 0), new Vector3(width, height, depth));
    }
    
    public static Vector2 FromPolar(double magnitude, double angle)
    {
        return new Vector2(
            (float) Math.Round(magnitude * Math.Cos(angle)), 
            (float) Math.Round(magnitude * Math.Sin(angle)));
    }
    
    public static Vector2 XY(this Vector3 vector)
    {
        return new Vector2(
            (float) vector.X, 
            (float) vector.Y);
    }
    
    public static Vector2 XZ(this Vector3 vector)
    {
        return new Vector2(
            (float) vector.X, 
            (float) vector.Y);
    }
    
    public static Vector2 YZ(this Vector3 vector)
    {
        return new Vector2(
            (float) vector.Y, 
            (float) vector.Z);
    }
    
    public static int XInt(this Vector2 vector)
    {
        return (int) Math.Round(vector.X);
    }
    
    public static int XInt(this Vector3 vector)
    {
        return (int) Math.Round(vector.X);
    }
    
    public static int YInt(this Vector2 vector)
    {
        return (int) Math.Round(vector.Y);
    }
    
    public static int YInt(this Vector3 vector)
    {
        return (int) Math.Round(vector.Y);
    }
    
    public static int ZInt(this Vector3 vector)
    {
        return (int) Math.Round(vector.Z);
    }
}