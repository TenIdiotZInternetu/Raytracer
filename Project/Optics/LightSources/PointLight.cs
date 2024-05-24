using System.Numerics;
using Newtonsoft.Json;
using OpenTK;
using rt004.SceneDefinition;
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

    public bool InShade(Scene scene, Intersection shadedIntersect)
    {
        foreach (var solid in scene.Solids)
        {
            if (solid == shadedIntersect.InnerSolid) continue;
            
            Ray shadowRay = new Ray(shadedIntersect.Position, GetLightVector(shadedIntersect.Position));
            Intersection? throwingIntersect = solid.GetRayIntersection(shadowRay);          // As in throwing shade
            if (throwingIntersect != null) return true;
        }

        return false;
    }
}