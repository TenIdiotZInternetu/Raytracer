using OpenTK.Mathematics;
using rt004.SceneDefinition.Solids;
using rt004.Utils;
using Void = rt004.SceneDefinition.Solids.Void;

namespace rt004.Optics;

public struct Intersection
{
    public Vector3 Position { get; init; }
    public Ray Ray { get; init; }
    public Vector3 ViewDirection => -Ray.Direction;

    public Material InnerMaterial;
    public Material OuterMaterial => Ray.ContainingMaterial;
    
    public float DistanceFromOrigin => Vector3.Distance(Position, Ray.Origin);
    public Vector3 SurfaceNormal { get; init; }
    
    public Intersection(Ray ray, float tParameter, Solid solid, Material innerMaterial)
    {
        Ray = ray;
        Position = ray.At(tParameter);

        // Assuming there are no nested solids within the scene
        if (innerMaterial == ray.ContainingMaterial)
        {
            SurfaceNormal = -solid.GetNormalAtPoint(Position);
            InnerMaterial = Material.Void;
        }
        else
        {
            SurfaceNormal = solid.GetNormalAtPoint(Position);
            InnerMaterial = innerMaterial;
        }
    }
    
    public Intersection Transform(Matrix4 transformation)
    {
        return new Intersection
        {
            Position = (Position.ToHomogenous() * transformation).To3d(),
            Ray = Ray.Transform(transformation),
            SurfaceNormal = (SurfaceNormal.ToHomogenous() * transformation).To3d().Normalized(),
            InnerMaterial = InnerMaterial,
        };
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