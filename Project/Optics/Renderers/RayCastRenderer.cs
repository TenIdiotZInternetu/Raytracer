using OpenTK;
using rt004.Optics.BRDF;
using rt004.SceneDefinition;
using rt004.Utils;
using Util;

namespace rt004.Optics.Renderers;

public class RayCastRenderer : IRenderer
{
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
        
        foreach (var ray in rayBatch.Rays)
        {
            if (!ray.Exists) continue;
            Intersection? intersection = _scene.FindIntersection(ray);

            if (intersection == null)
            {
                rayColors.Add(_scene.BackgroundColor);
                continue;
            }
            
            rayColors.Add(ContributeLights((Intersection)intersection));
        }

        Color3<Rgb> pixelColor = Colors.Average(rayColors);
        return new[] { pixelColor.X, pixelColor.Y, pixelColor.Z };
    }

    private Color3<Rgb> ContributeLights(Intersection intersection)
    {
        Color3<Rgb> color = _scene.AmbientColor;

        foreach (var light in _scene.LightSources)
        {
            if (light.InShade(_scene, intersection)) continue;

            float reflectance = _brdf.GetReflectance(light, intersection);
            color = color.Add(light.EmittedColor.Multiply(reflectance));
            color = color.Add(_brdf.GetDiffuseColor(light, intersection));
        }

        return color;
    }
}