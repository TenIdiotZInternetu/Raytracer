using OpenTK;
using OpenTK.Mathematics;
using rt004.SceneDefinition.Solids;
using rt004.Utils;
using Vector3 = OpenTK.Mathematics.Vector3;
using Void = rt004.SceneDefinition.Solids.Void;

namespace rt004.Optics;

public struct Ray
{
    public bool Exists => Direction != Vector3.Zero;
    public Vector3 Origin { get; private set; }
    public Vector3 Direction { get; private set; }
    public Solid ContainingSolid { get; private set; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction.Normalized();
        ContainingSolid = Solid.Void;
    }
    
    public Ray(Vector3 origin, Vector3 direction, Solid containingSolid)
    {
        Origin = origin;
        Direction = direction.Normalized();
        ContainingSolid = containingSolid;
    }

    public Vector3 At(float distanceT)
    {
        return Origin + distanceT * Direction;
    }

    public Ray Transform(Matrix4 transformation)
    {
        Vector4 homogeneousOrigin = Origin.ToHomogenous();
        Vector4 homogeneousDirection = Direction.ToHomogenous();
        
        Vector3 newOrigin = (homogeneousOrigin * transformation).To3d();
        Vector3 newDirection = ((homogeneousDirection * transformation).To3d() - newOrigin).Normalized();
        
        return new Ray(newOrigin, newDirection, ContainingSolid);
    }
}