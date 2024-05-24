using OpenTK.Mathematics;
using rt004.SceneDefinition.Solids;
using Void = rt004.SceneDefinition.Solids.Void;

namespace rt004.Optics;

public struct Intersection
{
    public Vector3 Position { get; init; }
    public Ray Ray { get; init; }
    public Vector3 ViewDirection => -Ray.Direction;

    public Solid InnerSolid { get; init; }
    public Solid OuterSolid => Ray.ContainingSolid;
    public Material InnerMaterial => InnerSolid.Material;
    public Material OuterMaterial => OuterSolid.Material;
    
    public float DistanceFromOrigin => Vector3.Distance(Position, Ray.Origin);
    public Vector3 SurfaceNormal { get; init; }
    
    public Intersection(Vector3 position, Ray ray, Solid solid)
    {
        Position = position;
        Ray = ray;
        Vector3 surfaceNormal = solid.GetNormalAtPoint(position);

        // Assuming there are no nested solids within the scene
        if (solid == ray.ContainingSolid)
        {
            SurfaceNormal = -surfaceNormal;
            InnerSolid = Solid.Void;
        }
        else
        {
            SurfaceNormal = surfaceNormal;
            InnerSolid = solid;
        }
    }
    
    public Vector3 GetReflection() 
    {
        float vnCosine = Vector3.Dot(ViewDirection, SurfaceNormal);
        return 2 * SurfaceNormal * vnCosine - ViewDirection;
    }

    public Vector3 GetRefraction()
    {
        float interfaceIndex = OuterMaterial.RefractiveIndex / InnerMaterial.RefractiveIndex;
        float vnCosine = Vector3.Dot(ViewDirection, SurfaceNormal);
        
        // angle between the opposite normal and the refracted ray
        float ntCosine2 = 1 - interfaceIndex * interfaceIndex * (1 - vnCosine * vnCosine);

        // total reflection
        if (ntCosine2 <= 0) return Vector3.Zero;
        
        Vector3 refractedVector = (interfaceIndex * vnCosine - MathF.Sqrt(ntCosine2)) * SurfaceNormal
                                  - interfaceIndex * ViewDirection;
        
        return refractedVector;
    }
}