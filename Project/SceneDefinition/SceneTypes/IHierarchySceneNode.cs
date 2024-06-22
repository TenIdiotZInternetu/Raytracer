using Newtonsoft.Json;
using OpenTK.Mathematics;
using rt004.Optics;
using rt004.SceneDefinition.Solids;

namespace rt004.SceneDefinition.SceneTypes;

public interface IHierarchySceneNode
{
    public Vector3 Translation { get; init; }
    public Vector3 Rotation { get; init; }
    public Vector3 Scale { get; init; }
}

public struct HierarchySceneInnerNode : IHierarchySceneNode
{
    [JsonProperty] public Vector3 Translation { get; init; }
    [JsonProperty] public Vector3 Rotation { get; init; }
    [JsonProperty] public Vector3 Scale { get; init; }
    
    [JsonProperty] public List<IHierarchySceneNode> Children { get; init; }
}

public struct HierarchySceneSolid : IHierarchySceneNode
{
    [JsonProperty] public Vector3 Translation { get; init; }
    [JsonProperty] public Vector3 Rotation { get; init; }
    [JsonProperty] public Vector3 Scale { get; init; }
    
    [JsonProperty] public Solid Solid { get; init; }
    [JsonProperty] public string MaterialName { get; init; }
}