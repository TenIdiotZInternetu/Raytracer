using System.Reflection;
using JsonSubTypes;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using rt004.Optics;

namespace rt004.SceneDefinition.Solids;

public abstract class Solid
{
    protected const float EPSILON = 1e-4f;

    [JsonProperty] public Vector3 Position { get; protected set; }
    [JsonProperty] public string MaterialTag { get; protected set; }
    public Material Material { get; set; }
    
    public static Void Void => new Void();
    
    
    protected bool _isInitialized = false;

    public abstract bool IntersectsRay(Ray ray);

    public abstract Intersection? GetRayIntersection(Ray ray);

    public abstract Vector2? GetUvCoordinates(Vector3 point);

    public abstract Vector3 GetNormalAtPoint(Vector3 point);
}