namespace rt004.Optics;

public struct RayBatch
{
    public bool IsNull => Rays == null;
    public Ray[] Rays { get; }
    public int PixelX { get; }
    public int PixelY { get; }
    
    public RayBatch(Ray[] rays, int pixelX, int pixelY)
    {
        Rays = rays;
        PixelX = pixelX;
        PixelY = pixelY;
    }
}