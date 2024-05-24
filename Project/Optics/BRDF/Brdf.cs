using JsonSubTypes;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;
using rt004.Optics.LightSources;
using rt004.SceneDefinition;

namespace rt004.Optics.BRDF;

public abstract class Brdf
{
    public abstract float GetReflectance(Vector3 lightDirection, Intersection point);
    public abstract float GetTransmittance(Vector3 lightDirection, Intersection point);

    public float GetReflectance(ILightSource light, Intersection point)
    {
        return GetReflectance(light.GetLightVector(point.Position), point);
    }

    public float GetTransmittance(ILightSource light, Intersection point)
    {
        return GetTransmittance(light.GetLightVector(point.Position), point);
    }
    
    public virtual Color3<Rgb> GetDiffuseColor(ILightSource light, Intersection point)
    {
        Vector3 lightDirection = light.GetLightVector(point.Position);
        
        // Alpha is the angle between the light direction and the surface normal
        float alphaCosine = Vector3.Dot(lightDirection, point.SurfaceNormal);
        alphaCosine = MathF.Max(alphaCosine, 0);
        float colorIntensity = light.Intensity * point.InnerMaterial.KDiffuse * alphaCosine;
        
        Color3<Rgb> color = point.InnerMaterial.DiffuseColor.Multiply(colorIntensity);
        return color;
    }
}