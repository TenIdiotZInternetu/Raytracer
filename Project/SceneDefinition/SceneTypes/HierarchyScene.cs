using Newtonsoft.Json;
using OpenTK;
using rt004.Optics;
using rt004.Optics.LightSources;
using rt004.Utils;

namespace rt004.SceneDefinition.SceneTypes;

public class HierarchyScene : IScene
{
    [JsonProperty] public Color3<Rgb> BackgroundColor { get; init; }
    [JsonProperty] public Camera Camera { get; init; }
    
    
    
    public void Initialize(Configuration config)
    {
        throw new NotImplementedException();
    }

    public Intersection? FindIntersection(Ray ray)
    {
        throw new NotImplementedException();
    }

    public bool IntersectsWithScene(Ray ray)
    {
        throw new NotImplementedException();
    }

    public ILightSource[] GetLights()
    {
        throw new NotImplementedException();
    }
}