using OpenTK;
using rt004.Optics;
using rt004.Optics.LightSources;
using rt004.Utils;

namespace rt004.SceneDefinition.SceneTypes;

public interface IScene
{
    public Color3<Rgb> BackgroundColor { get; init; }
    public Camera Camera { get; init; }
    
    public void Initialize(Configuration config);
    public Intersection? FindIntersection(Ray ray);
    public bool IntersectsWithScene(Ray ray);
    public ILightSource[] GetLights();
}