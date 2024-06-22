using Newtonsoft.Json;
using OpenTK.Mathematics;
using rt004.Optics;
using rt004.Utils;

namespace rt004.SceneDefinition.Solids;

public class Sphere : Solid
{
    [JsonProperty] public float Radius { get; private set; }

    public Sphere()
    {
        Radius = 1;
    }
    
    public override bool IntersectsRay(Ray ray)
    {
        if (!_isInitialized) Initialize();
        
        Vector3 relativeRayPos = ray.Origin - Position;
        
        return 0 > QuadraticFormula.GetDeterminant(
            Vector3.Dot(relativeRayPos, relativeRayPos),
            Vector3.Dot(relativeRayPos, ray.Direction),
            Vector3.Dot(ray.Direction, ray.Direction) - Radius * Radius);
    }

    public override float FindIntersectionParameter(Ray ray)
    {
        if (!_isInitialized) Initialize();

        Vector3 relativeRayPos = ray.Origin - Position;

        var solutions = QuadraticFormula.Solve(
            Vector3.Dot(ray.Direction, ray.Direction),
            2 * Vector3.Dot(relativeRayPos, ray.Direction),
            Vector3.Dot(relativeRayPos, relativeRayPos) - Radius * Radius);

        if (solutions == null) return MISS;
        
        float closer = MathF.Min(solutions.Root1, solutions.Root2);
        float further = MathF.Max(solutions.Root1, solutions.Root2);
        
        float distance = closer;
        if (distance < EPSILON) distance = further;
        if (distance < EPSILON) return MISS;

        return distance;
    }

    public override Vector2? GetUvCoordinates(Vector3 point)
    {
        if (!_isInitialized) Initialize();
        throw new NotImplementedException();
    }
    
    public override Vector3 GetNormalAtPoint(Vector3 point)
    {
        return (point - Position).Normalized();
    }

    private void Initialize()
    {
        _isInitialized = true;
    }

}