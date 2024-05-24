using OpenTK.Mathematics;

namespace rt004.Optics.Samplers;

public class JitteringSampler : ISampler
{
    private static UniformSampler _uniform = new UniformSampler();
    private static RandomSampler _random = new RandomSampler();
    
    public Vector2[] GetSamples(int samplesPerPixel)
    {
        float samplesPerRow = MathF.Sqrt(samplesPerPixel);
        float step = 1.0f / samplesPerRow;
        
        var uniformSamples = _uniform.GetSamples(samplesPerPixel);
        var randomSamples = _random.GetSamples(samplesPerPixel);
        var zip = uniformSamples.Zip(randomSamples);

        var samples = new Vector2[uniformSamples.Length];
        
        int i = 0;
        foreach (var (uni, rand) in zip)
        {
            samples[i++] = uni + rand * step;
        }
        
        return samples;
    }
}