using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;
using rt004.Optics;
using rt004.Optics.LightSources;
using rt004.SceneDefinition.Solids;
using rt004.Utils;
using Void = rt004.SceneDefinition.Solids.Void;

namespace rt004.SceneDefinition.SceneTypes;

public class HierarchyScene : IScene
{
    private const float EPSILON = 1e-3f;

    private record struct SceneObject(
        Solid Solid,
        string MaterialName,
        Matrix4 Transformation,
        Matrix4 InverseTransformation);
    
    [JsonProperty] public Color3<Rgb> BackgroundColor { get; init; }
    [JsonProperty] public Camera Camera { get; init; }
    [JsonProperty] public List<ILightSource> LightSources { get; init; }
    
    [JsonProperty] public List<IHierarchySceneNode> Tree { get; init; }

    private List<SceneObject> _objects = new();
    private Solid[] _primitives;
    
    
    public void Initialize(Configuration config)
    {
        ConstructObjects();
        AssignMaterials(config.Materials);
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
            Intersection? intersection = obj.Solid.GetRayIntersection(transformedRay);
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

    private void ConstructObjects()
    {
        foreach(var node in Tree)
        {
            ConstructObjects(node, Matrix4.Identity);
        }
    }

    private void ConstructObjects(IHierarchySceneNode node, Matrix4 parentTransform)
    {
        Matrix4 transformation = ConstructTransform(node) * parentTransform;
        Matrix4 inverseTransform = transformation.Inverted();
        
        if (node is HierarchySceneSolid solidNode)
        {
            _objects.Add(new SceneObject(solidNode.Solid, solidNode.MaterialName, transformation, inverseTransform));
        }
        else if (node is HierarchySceneInnerNode innerNode)
        {
            foreach (var child in innerNode.Children)
            {
                ConstructObjects(child, transformation);
            }
        }
    }

    private Matrix4 ConstructTransform(IHierarchySceneNode node)
    {
        Matrix4 rotationX = Matrix4.CreateRotationX(node.Rotation.X);
        Matrix4 rotationY = Matrix4.CreateRotationY(node.Rotation.Y);
        Matrix4 rotationZ = Matrix4.CreateRotationZ(node.Rotation.Z);
        
        Matrix4 scale = Matrix4.CreateScale(node.Scale);
        Matrix4 translation = Matrix4.CreateTranslation(node.Translation);
        
        return rotationX * rotationY * rotationZ * scale * translation;
    }
    
    private void AssignMaterials(List<Material> materials)
    {
        foreach (var obj in _objects)
        {
            obj.Solid.Material = materials.Find(material => material.Name == obj.MaterialName);
        }
    }
}