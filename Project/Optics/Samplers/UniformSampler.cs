using OpenTK.Mathematics;

namespace rt004.Optics.Samplers;

public class UniformSampler : ISampler
{
    public Vector2[] GetSamples(int samplesPerPixel)
    {
        float step = 1.0f / MathF.Sqrt(samplesPerPixel);
        int samplesPerRow = (int)MathF.Floor(MathF.Sqrt(samplesPerPixel));
        
        Vector2[] samples = new Vector2[samplesPerRow * samplesPerRow];
        
        for (int i = 0; i < samplesPerRow; i++)
        {
            float x = i * step;
            
            for (int j = 0; j < samplesPerRow; j++)
            {
                float y = j * step;
                samples[i] = new Vector2(x, y);
            }
        }

        return samples;
    }
}