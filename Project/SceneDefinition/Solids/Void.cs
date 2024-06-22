using OpenTK.Mathematics;
using rt004.Optics;

namespace rt004.SceneDefinition.Solids;

public class Void : Solid
{
    public Void()
    {
        Position = Vector3.Zero;
        MaterialTag = "--";
        Material = Material.Void;
        
    }
    
    public override bool IntersectsRay(Ray ray) => false;
    public override float FindIntersectionParameter(Ray ray) => MISS;
    public override Vector2? GetUvCoordinates(Vector3 point) => null;
    public override Vector3 GetNormalAtPoint(Vector3 point) => Vector3.Zero;
}