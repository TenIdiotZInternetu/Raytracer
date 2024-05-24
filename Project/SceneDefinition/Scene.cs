using Newtonsoft.Json;
using OpenTK;
using rt004.Optics;
using rt004.Optics.BRDF;
using rt004.Optics.LightSources;
using rt004.SceneDefinition.Solids;
using rt004.Utils;
using Util;

namespace rt004.SceneDefinition;

public class Scene
{
    private const float EPSILON = 1e-3f;
    [JsonProperty] public Color3<Rgb> BackgroundColor { get; init; }
    [JsonProperty] public Color3<Rgb> AmbientColor { get; init; }
    
    [JsonProperty] public Camera Camera { get; init; }
    [JsonProperty] public List<ILightSource> LightSources { get; init; }
    [JsonProperty] public List<Solid> Solids { get; init; }

    public void Iniitialize(Configuration config)
    {
        AssignMaterials(config.Materials);
    }
    
    public Intersection? FindIntersection(Ray ray)
    {
        Intersection? closestIntersection = null;
            
        foreach (var solid in Solids)
        {
            Intersection? intersection = solid.GetRayIntersection(ray);
            if (intersection == null) continue;
                
            if (intersection?.DistanceFromOrigin < closestIntersection?.DistanceFromOrigin ||
                closestIntersection == null)
            {
                closestIntersection = intersection;
            }
        }
        
        return closestIntersection;
    }
    
    private void AssignMaterials(List<Material> materials)
    {
        foreach (var solid in Solids)
        {
            solid.Material = materials.Find(material => material.Name == solid.MaterialTag!);
        }
    }
}