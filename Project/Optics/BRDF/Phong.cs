using OpenTK;
using rt004.Optics.LightSources;
using rt004.SceneDefinition;
using rt004.Utils;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace rt004.Optics.BRDF;


public class Phong : Brdf
{
    public override float GetReflectance(Vector3 lightDirection, Intersection point)
    {
        // Alpha is the angle between the light direction and the surface normal
        float alphaCosine = Vector3.Dot(lightDirection, point.SurfaceNormal);
        alphaCosine = MathF.Max(alphaCosine, 0);
        Vector3 reflectionDirection = 2 * point.SurfaceNormal * alphaCosine - lightDirection;

        // Beta is the angle between the reflected light and the direction of spectator
        float betaCosine = Vector3.Dot(reflectionDirection, -point.Ray.Direction);
        betaCosine = MathF.Max(betaCosine, 0);

        float reflectionIntensity = MathF.Pow(betaCosine, point.InnerMaterial.Shininess);
        float reflectance = point.InnerMaterial.KSpecular * reflectionIntensity;

        return reflectance;
    }

    public override float GetTransmittance(Vector3 lightDirection, Intersection point)
    {
        return point.InnerMaterial.KTransparent;
    }
}