using OpenTK.Mathematics;

namespace rt004.Optics.Samplers;

// Algorithm taken from https://www.pbr-book.org/3ed-2018/Sampling_and_Reconstruction/The_Halton_Sampler
// Implemented for base of 2
public class HammersleySampler : ISampler
{
    const float _2toMinus32 = 2.3283064e-10f;
    public Vector2[] GetSamples(int samplesPerPixel)
    {
        var samples = new Vector2[samplesPerPixel];
        
        for (uint i = 0; i < samplesPerPixel; i++)
        {
            float x = i / (float) samplesPerPixel;
            float y = ReverseBits(i) * _2toMinus32;
            samples[i] = new Vector2(x, y);
        }

        return samples;
    }

    private uint ReverseBits(uint n)
    {
        n = (n << 16) | (n >> 16);
        n = ((n & 0x00ff00ff) << 8) | ((n & 0xff00ff00) >> 8);
        n = ((n & 0x0f0f0f0f) << 4) | ((n & 0xf0f0f0f0) >> 4);
        n = ((n & 0x33333333) << 2) | ((n & 0xcccccccc) >> 2);
        n = ((n & 0x55555555) << 1) | ((n & 0xaaaaaaaa) >> 1);
        
        return n;
    }
}