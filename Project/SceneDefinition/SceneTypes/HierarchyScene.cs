using System.Reflection;
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
        Material Material,
        Matrix4 Transformation,
        Matrix4 InverseTransformation);
    
    [JsonProperty] public Color3<Rgb> BackgroundColor { get; init; }
    [JsonProperty] public Camera Camera { get; init; }
    [JsonProperty] public List<ILightSource> LightSources { get; init; }
    
    [JsonProperty] public List<IHierarchySceneNode> Tree { get; init; }

    private List<SceneObject> _objects = new();
    
    private Dictionary<string, Solid> _primitives = new();
    private Dictionary<string, Material> _materials = new();
    
    
    public void Initialize(Configuration config)
    {
        AddMaterials(config.Materials);
        ConstructPrimitives();
        ConstructObjects();
    }

    public Intersection? FindIntersection(Ray ray)
    {
        float closestT = Solid.MISS;
        SceneObject? intersectedObject = null;
        Intersection? intersection = null;
            
        foreach (var obj in _objects)
        {
            Ray transformedRay = ray.Transform(obj.Transformation);
            float t = obj.Solid.FindIntersectionParameter(transformedRay);
            if (t == Solid.MISS) continue;
            
            if (t < closestT || t > EPSILON)
            {
                closestT = t;
                intersectedObject = obj;
                intersection = new Intersection(transformedRay, t, obj.Solid, obj.Material);
            }
        }
        
        if (intersection == null) return null;
        return intersection.Value.Transform(intersectedObject!.Value.InverseTransformation);
    }

    public bool IntersectsWithScene(Ray ray)
    {
        foreach (var obj in _objects)
        {
            Ray transformedRay = ray.Transform(obj.Transformation);
            float t = obj.Solid.FindIntersectionParameter(transformedRay);

            if (t > EPSILON)
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
    
    private void AddMaterials(List<Material> materials)
    {
        foreach (var material in materials)
        {
            _materials.Add(material.Name, material);
        }
    }
    
    private void ConstructPrimitives()
    {
        Type solidBase = typeof(Solid);
        Assembly assembly = solidBase.Assembly;
        
        Solid[] primitives = assembly.GetTypes()
            .Where(type => type.IsSubclassOf(solidBase))
            .Select(type => (Solid) Activator.CreateInstance(type)!)
            .ToArray();

        foreach (var primitive in primitives)
        {
            _primitives.Add(primitive.GetType().Name, primitive);
        }
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
            Solid solid = _primitives[solidNode.SolidType];
            Material material = _materials[solidNode.MaterialName];
            _objects.Add(new SceneObject(solid, material, transformation, inverseTransform));
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
        
        Matrix4 scale = (node.Scale == Vector3.Zero) ? Matrix4.Identity : Matrix4.CreateScale(node.Scale);
        Matrix4 translation = Matrix4.CreateTranslation(node.Translation);
        
        return rotationX * rotationY * rotationZ * scale * translation;
    }
}