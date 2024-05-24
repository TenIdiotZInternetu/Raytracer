using Newtonsoft.Json;
using OpenTK;
using OpenTK.Mathematics;

namespace rt004.Optics;

public class Material
{
    [JsonProperty] public string Name { get; private set; }
    [JsonProperty] public Color3<Rgb> DiffuseColor { get; private set; }
    
    [JsonProperty] public float KDiffuse { get; private set; }
    [JsonProperty] public float KSpecular { get; private set; }
    [JsonProperty] public float KTransparent { get; private set; }
    [JsonProperty] public float Shininess { get; private set; }
    [JsonProperty] public float RefractiveIndex { get; private set; }

    public float Roughness => 1 - KSpecular;

    private Material()
    {
        Name = "__";
        DiffuseColor = new(0, 0, 0);
        KDiffuse = 0;
        KSpecular = 0;
        KTransparent = 1;
        Shininess = 0;
        RefractiveIndex = 1;
    }

    public static Material Void { get; } = new Material();
}