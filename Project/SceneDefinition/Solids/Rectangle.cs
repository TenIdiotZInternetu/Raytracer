using Newtonsoft.Json;
using OpenTK.Mathematics;
using rt004.Optics;

namespace rt004.SceneDefinition.Solids;

public class Rectangle : Solid
{
    [JsonProperty] public Vector3 Normal { get; private set; }
    [JsonProperty] public Vector3 WidthAxis { get; private set; }
    
    [JsonProperty] public float Width { get; private set; }
    [JsonProperty] public float Height { get; private set; }
    
    private Vector3 _heightAxis;
    private Vector3 _corner;
    
    // As in the parametric form: x * xn + y * yn + z * zn + D = 0
    private float _dParameter;

    public override bool IntersectsRay(Ray ray)
    {
        if (!_isInitialized) Initialize();
        return GetRayIntersection(ray) != null;
    }
    
    public override Intersection? GetRayIntersection(Ray ray)
    {
        if (!_isInitialized) Initialize();

        float rayIntersectionDistance = 
            -(Vector3.Dot(ray.Origin, Normal) + _dParameter)
            / Vector3.Dot(ray.Direction, Normal);
        
        if (rayIntersectionDistance < EPSILON) return null;
        
        Vector3 intersectionPoint = ray.At(rayIntersectionDistance);

        if (GetUvCoordinates(intersectionPoint) == null) return null;
        
        return new Intersection(intersectionPoint, ray, this);
    }
    
    public override Vector2? GetUvCoordinates(Vector3 point)
    {
        if (!_isInitialized) Initialize();

        Vector3 relativePoint = point - _corner;
        
        float u = Vector3.Dot(relativePoint, WidthAxis) / Width;
        float v = Vector3.Dot(relativePoint, _heightAxis) / Height;
        
        if (u is >1 or <0 || v is >1 or <0) return null;
        
        return new Vector2(u, v);
    }
    
    public override Vector3 GetNormalAtPoint(Vector3 point)
    {
        return Normal;
    }

    private void Initialize()
    {
        WidthAxis = WidthAxis.Normalized();
        Normal = Normal.Normalized();
        
        if (Vector3.Dot(Normal, WidthAxis) > 1e06)
        {
            throw new ArgumentException("Normal and WidthAxis vectors must be orthogonal");
        }
        
        _heightAxis = Vector3.Cross(WidthAxis, Normal).Normalized();
        _dParameter = -Vector3.Dot(Position, Normal);
        _corner = Position - WidthAxis * Width / 2 - _heightAxis * Height / 2;
        
        _isInitialized = true;
    }
}