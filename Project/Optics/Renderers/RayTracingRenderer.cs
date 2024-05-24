using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;
using rt004.Optics.BRDF;
using rt004.Optics.LightSources;
using rt004.SceneDefinition;
using rt004.Utils;
using Util;

namespace rt004.Optics.Renderers;

public class RayTracingRenderer : IRenderer
{
    public const float MIN_INTENSITY = 0.01f;
    private static readonly Color3<Rgb> BLACK = new(0, 0, 0);
    [JsonProperty] public int MaxDepth { get; init; } = 10;
    
    private Scene _scene;
    private Brdf _brdf;
    
    public FloatImage Render(Scene scene, Brdf brdf)
    {
        _scene = scene;
        _brdf = brdf;
        
        var camera = scene.Camera;
        
        var image = new FloatImage(camera.ResolutionWidth, camera.ResolutionHeight, 3);
        List<RayBatch> rayBatches = camera.GenerateRays();

        foreach (var batch in rayBatches)
        {
            float[] color = GetPixelColor(batch);
            image.PutPixel(batch.PixelX, batch.PixelY, color);
        }

        return image;
    }
    
    private float[] GetPixelColor(RayBatch rayBatch)
    {
        List<Color3<Rgb>> rayColors = new();

        // if (rayBatch.PixelX == 362 && rayBatch.PixelY == 114)
        //     Console.WriteLine("Debugging");
        
        foreach (var ray in rayBatch.Rays)
        {
            if (!ray.Exists) continue;
            rayColors.Add(TraceRay(ray).EmittedColor);
        }

        Color3<Rgb> pixelColor = Colors.Average(rayColors);
        return new[] { pixelColor.X, pixelColor.Y, pixelColor.Z };
    }

    private RayTracedLight TraceRay(Ray ray, float intensity = 1, int depth = 0)
    {
        if (depth >= MaxDepth || intensity < MIN_INTENSITY)
        {
            return new RayTracedLight(-ray.Direction, _scene.BackgroundColor, 0);
        }
        
        Intersection? intersection = _scene.FindIntersection(ray);

        if (intersection == null)
        {
            return new RayTracedLight(-ray.Direction, _scene.BackgroundColor, intensity);
        }
        
        Intersection point = intersection.Value;
        
        Color3<Rgb> resColor = BLACK;
        
        resColor = resColor.Add(ContributeLights(point));
        resColor = resColor.Add(ContributeReflections(point, intensity, depth));
        resColor = resColor.Add(ContributeRefractions(point, intensity, depth));

        return new RayTracedLight(point.ViewDirection, resColor, intensity);
    }

    private Color3<Rgb> ContributeLights(Intersection point)
    {
        Color3<Rgb> color = BLACK;

        foreach (var light in _scene.LightSources)
        {
            if (light.InShade(_scene, point)) continue;
            color = color.Add(_brdf.GetDiffuseColor(light, point));
        }

        return color;
    }

    private Color3<Rgb> ContributeReflections(Intersection point, float intensity, int depth)
    {
        Vector3 reflection = point.GetReflection();
        float reflectance = _brdf.GetReflectance(reflection, point);
        
        Ray reflectedRay = new Ray(point.Position, reflection, point.OuterSolid);
        RayTracedLight reflectedLight = TraceRay(reflectedRay, reflectance * intensity, depth + 1);

        Color3<Rgb> resColor = _brdf.GetDiffuseColor(reflectedLight, point);
        resColor = resColor.Add(reflectedLight.EmittedColor);
        return resColor;
    }
    
    private Color3<Rgb> ContributeRefractions(Intersection point, float intensity, int depth)
    {
        Vector3 refraction = point.GetRefraction();
        bool totalReflection = refraction == Vector3.Zero;

        if (totalReflection) return BLACK;
        
        float transmittance = _brdf.GetTransmittance(refraction, point);
        Ray refractedRay = new Ray(point.Position, refraction, point.InnerSolid);
        RayTracedLight refractedLight = TraceRay(refractedRay, transmittance * intensity, depth + 1);
        
        return refractedLight.EmittedColor;
    }
}