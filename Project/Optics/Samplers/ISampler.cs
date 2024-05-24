using OpenTK.Mathematics;

namespace rt004.Optics.Samplers;

public interface ISampler
{
    public Vector2[] GetSamples(int samplesPerPixel);
}