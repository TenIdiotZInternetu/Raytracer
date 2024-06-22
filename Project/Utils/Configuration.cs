using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using rt004.Optics;
using rt004.Optics.BRDF;
using rt004.Optics.LightSources;
using rt004.Optics.Renderers;
using rt004.Optics.Samplers;
using rt004.SceneDefinition;
using rt004.SceneDefinition.SceneTypes;
using rt004.SceneDefinition.Solids;

namespace rt004.Utils;

public class Configuration
{
    [JsonProperty] public string Author { get; init; }
    [JsonProperty] public IRenderer Renderer { get; init; }
    [JsonProperty] public Brdf Brdf { get; init; }
    [JsonProperty] public IScene Scene { get; init; }
    [JsonProperty] public List<Material> Materials { get; init; }
    
    public static Configuration Load(string jsonFileName, bool trace = false)
    {
        var settings = new JsonSerializerSettings();
        RegisterSubtypes(settings);
        var traceWriter = new MemoryTraceWriter();
        settings.TraceWriter = traceWriter;
        
        string jsonString = File.ReadAllText(jsonFileName);
        var configuration = JsonConvert.DeserializeObject<Configuration>(jsonString, settings)!;

        if (trace)
        {
            Console.WriteLine(traceWriter);
        }

        configuration.Scene.Initialize(configuration);
        configuration.Renderer.Initialize(configuration);
        return configuration;
    }

    private static void RegisterSubtypes(JsonSerializerSettings settings)
    {
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<IRenderer>("Tag")
            .RegisterSubtype<RayCastRenderer>("RAYCAST")
            .RegisterSubtype<RayTracingRenderer>("RAYTRACE")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<Brdf>("Tag")
            .RegisterSubtype<Phong>("PHONG")
            .RegisterSubtype<MicrofacetBrdf>("MICROFACET")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<ILightSource>("Tag")
            .RegisterSubtype<PointLight>("POINT")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<Solid>("Tag")
            .RegisterSubtype<Sphere>("SPHERE")
            .RegisterSubtype<Rectangle>("RECTANGLE")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<ISampler>("Tag")
            .RegisterSubtype<UniformSampler>("UNIFORM")
            .RegisterSubtype<RandomSampler>("RANDOM")
            .RegisterSubtype<JitteringSampler>("JITTERING")
            .RegisterSubtype<HammersleySampler>("HAMMERSLEY")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<IScene>("Tag")
            .RegisterSubtype<HierarchyScene>("TREE")
            .SerializeDiscriminatorProperty()
            .Build());
        
        settings.Converters.Add(JsonSubtypesConverterBuilder
            .Of<IHierarchySceneNode>("Tag")
            .RegisterSubtype<HierarchySceneInnerNode>("INNER")
            .RegisterSubtype<HierarchySceneSolid>("SOLID")
            .SerializeDiscriminatorProperty()
            .Build());
    }
}