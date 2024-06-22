using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;
using rt004.Optics;
using rt004.Optics.LightSources;
using rt004.SceneDefinition.Solids;
using rt004.Utils;

namespace rt004.SceneDefinition.SceneTypes;

public class HierarchyScene : IScene
{
    private const float EPSILON = 1e-3f;
    private struct SceneObject
    {
        public Solid Solid;
        public Material Material;
        public Matrix4 Transformation;
        public Matrix4 InverseTransformation;
    }
    
    [JsonProperty] public Color3<Rgb> BackgroundColor { get; init; }
    [JsonProperty] public Camera Camera { get; init; }
    [JsonProperty] public List<ILightSource> LightSources { get; init; }
    
    [JsonProperty] public List<IHierarchySceneNode> Tree { get; init; }

    private List<SceneObject> _objects = new();
    
    
    public void Initialize(Configuration config)
    {
        throw new NotImplementedException();
    }

    public Intersection? FindIntersection(Ray ray)
    {
        Intersection? closestIntersection = null;
        SceneObject? closestObject = null;
            
        foreach (var obj in _objects)
        {
            Ray transformedRay = ray.Transform(obj.Transformation);
            Intersection? intersection = obj.Solid.GetRayIntersection(transformedRay);
            if (intersection == null) continue;
            
            if (intersection?.DistanceFromOrigin < closestIntersection?.DistanceFromOrigin ||
                closestIntersection == null)
            {
                closestIntersection = intersection;
                closestObject = obj;
            }
        }
        
        return ToCanonical(closestIntersection, closestObject);
    }

    public bool IntersectsWithScene(Ray ray)
    {
        foreach (var obj in _objects)
        {
            Ray transformedRay = ray.Transform(obj.Transformation);
            Intersection? intersection = obj.Solid.GetRayIntersection(ray);
            if (intersection is { DistanceFromOrigin: > EPSILON })
            {
                return true;
            }
        }

        return false;
    }

    public ILightSource[] GetLights()
    {
        return LightSources.ToArray();
    }

    private Intersection? ToCanonical(Intersection? nonCanonical, SceneObject? intersectedObject)
    {
        if (nonCanonical == null) return null;

        Intersection point = nonCanonical.Value;
        SceneObject obj = intersectedObject!.Value;

        Vector3 canonicalPos = (point.Position.ToHomogenous() * obj.InverseTransformation).To3d();
        Ray canonicalRay = point.Ray.Transform(obj.InverseTransformation);
        return new Intersection(canonicalPos, canonicalRay, obj.Solid);
    }
}