using Newtonsoft.Json;
using JsonSubTypes;
using rt004.Optics.BRDF;
using rt004.SceneDefinition;
using rt004.Utils;
using Util;

namespace rt004.Optics.Renderers;

public interface IRenderer
{
    public void Initialize(Configuration config);
    public float[] GetPixelColor(RayBatch rayBatch);
}