using OpenTK.Mathematics;

namespace rt004.Optics.Samplers;

public class RandomSampler : ISampler
{
    private static readonly Random Random = new Random();
    
    public Vector2[] GetSamples(int samplesPerPixel)
    {
        var samples = new Vector2[samplesPerPixel];
        
        for (int i = 0; i < samplesPerPixel; i++)
        {
            float rayYOffset = (float) Random.NextDouble();
            float rayXOffset = (float) Random.NextDouble();
            
            samples[i] = new Vector2(rayXOffset, rayYOffset);
        }

        return samples;
    }
}