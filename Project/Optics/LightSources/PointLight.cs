using System.Numerics;
using Newtonsoft.Json;
using OpenTK;
using rt004.SceneDefinition;
using rt004.SceneDefinition.SceneTypes;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace rt004.Optics.LightSources;

public class PointLight : ILightSource
{
    [JsonProperty] public Color3<Rgb> BaseColor { get; init; }
    [JsonProperty] public float Intensity { get; init; }
    [JsonProperty] public Vector3 Position { get; init; }

    public Vector3 GetLightVector(Vector3 point)
    {
        return (Position - point).Normalized();
    }

    public bool InShade(IScene scene, Intersection shadedIntersect)
    {
        Ray shadowRay = new Ray(shadedIntersect.Position, GetLightVector(shadedIntersect.Position));
        return scene.IntersectsWithScene(shadowRay);
    }
}