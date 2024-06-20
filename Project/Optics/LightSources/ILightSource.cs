using JsonSubTypes;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;
using rt004.SceneDefinition;
using rt004.SceneDefinition.SceneTypes;

namespace rt004.Optics.LightSources;

public interface ILightSource
{
    public Color3<Rgb> BaseColor { get; init; }
    public float Intensity { get; init; }
    
    public Color3<Rgb> EmittedColor => BaseColor.Multiply(Intensity);
    
    public Vector3 GetLightVector(Vector3 point);
    public bool InShade(IScene scene, Intersection shadedIntersect);
}