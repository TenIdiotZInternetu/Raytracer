using System.Runtime.InteropServices.ComTypes;
using rt004.Optics;
using rt004.Optics.BRDF;
using rt004.Optics.Renderers;
using rt004.SceneDefinition;
using rt004.Utils;
using Util;

namespace rt004;

public static class ImageProcessor
{
    private const int CHUNK_SIZE = 16;
    
    private static FloatImage _image;
    private static IRenderer _renderer;
    
    public static FloatImage Render(Configuration config, bool processParallel = true)
    {
        Camera camera = config.Scene.Camera; 
        RayBatch[,] rayBatches = camera.GenerateRays();
        
        _renderer = config.Renderer;
        _image = new FloatImage(camera.ResolutionWidth, camera.ResolutionHeight, 3);
        
        if (processParallel)
        {
            ProcessRaysParallel(rayBatches);
        }
        else
        {
            ProcessRays(rayBatches);
        }

        return _image;
    }
    
    private static void ProcessRays(RayBatch[,] rayBatches)
    {
        foreach (var rayBatch in rayBatches)
        {
            float[] color = _renderer.GetPixelColor(rayBatch);
            _image.PutPixel(rayBatch.PixelX, rayBatch.PixelY, color);
        }
    }

    private static void ProcessRaysParallel(RayBatch[,] rayBatches)
    {
        RayBatch[][] pixelChunks = Parallelism.Chunkify2D(rayBatches, CHUNK_SIZE);
        
        Parallel.For(0, pixelChunks.Length, i =>
        {
            RayBatch[] chunk = pixelChunks[i];

            foreach (var batch in chunk)
            {
                if (batch.IsNull) continue;
                float[] color = _renderer.GetPixelColor(batch);
                _image.PutPixel(batch.PixelX, batch.PixelY, color);
            }
        });
    }
    
}