using Newtonsoft.Json;
using JsonSubTypes;
using rt004.Optics.BRDF;
using rt004.SceneDefinition;
using Util;

namespace rt004.Optics.Renderers;

public interface IRenderer
{
    public FloatImage Render(Scene scene, Brdf brdf);
}