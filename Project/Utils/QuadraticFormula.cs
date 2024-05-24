namespace rt004.Utils;

public static class QuadraticFormula
{
    public record Solution(float Root1, float Root2);

    public static float GetDeterminant(float a, float b, float c)
    {
        return  b * b - 4 * a * c;
    }
    
    public static Solution? Solve(float a, float b, float c)
    {
        float determinant = GetDeterminant(a, b, c);

        if (determinant < 0) return null;
        
        double root1 = (-b - Math.Sqrt(determinant)) / 2 * a;
        double root2 = (-b + Math.Sqrt(determinant)) / 2 * a;

        return new Solution((float)root1, (float)root2);
    }
}