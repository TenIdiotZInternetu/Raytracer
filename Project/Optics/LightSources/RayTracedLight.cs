using OpenTK;
using OpenTK.Mathematics;
using rt004.SceneDefinition;

namespace rt004.Optics.LightSources;

public struct RayTracedLight : ILightSource
{
    public Vector3 Direction;
    public Color3<Rgb> BaseColor { get; init; }
    public float Intensity { get; init; }
    public Color3<Rgb> EmittedColor => BaseColor.Multiply(Intensity);

    
    public RayTracedLight(Vector3 direction, Color3<Rgb> color, float intensity = 1)
    {
        Direction = direction.Normalized();
        BaseColor = color;
        Intensity = intensity;
    }

    public Vector3 GetLightVector(Vector3 point)
    {
        return -Direction;
    }

    public bool InShade(Scene scene, Intersection shadedIntersect)
    {
        return false;
    }
}