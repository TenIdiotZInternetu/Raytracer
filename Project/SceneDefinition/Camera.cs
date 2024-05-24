using Newtonsoft.Json;
using OpenTK.Mathematics;
using rt004.Optics;
using rt004.Optics.Samplers;
using rt004.Utils;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace rt004.SceneDefinition;

public class Camera
{
    [JsonProperty] public Vector3 Position { get; private set; }
    
    [JsonProperty] public Vector3 Up { get; private set; }
    [JsonProperty] public Vector3 Forward { get; private set; }
    
    [JsonProperty] public float ScreenDistance { get; private set; }
    [JsonProperty] public float ViewingDistance { get; private set; }
    [JsonProperty] public float Fov { get; private set; }

    [JsonProperty] public int ResolutionWidth { get; private set; } = 600;
    [JsonProperty] public int ResolutionHeight { get; private set; } = 480;
    [JsonProperty] public int SamplesPerPixel { get; private set; } = 50;
    [JsonProperty] public ISampler Sampler { get; private set; } = new RandomSampler();

    private bool _isInitialized = false;

    private Vector3 _right;
    
    private float _screenWidth;
    private float _screenHeight;
    private Vector2 _screenCenter;

    private float _fovRadians;
    private float _resolutionPerUnit;
    private Vector2[] _samples;
    
    private Matrix3 _viewTransformation;

    public void SetResolution(int width, int height)
    {
        ResolutionWidth = width;
        ResolutionHeight = height;
        _isInitialized = false;
    }
    
    public void SetSamples(int samplesPerPixel)
    {
        SamplesPerPixel = samplesPerPixel;
    }
    
    public List<RayBatch> GenerateRays()
    {
        var rayBatches = new List<RayBatch>();

        if (!_isInitialized) Initialiaze();
        
        foreach (Vector2 pixel in VectorHelper.IterateRectangle(ResolutionWidth, ResolutionHeight))
        {
            Ray[] rays = GenerateRaysInPixel(pixel);
            
            int screenSpaceY = ResolutionHeight - pixel.YInt();             // Since (0, 0) is the top left corner of the screen
            var batch = new RayBatch(rays, pixel.XInt(), screenSpaceY);
            rayBatches.Add(batch);
        }

        return rayBatches;
    }
    
    private void Initialiaze()
    {
        if (Vector3.Dot(Forward, Up) > 1e06)
        {
            throw new ArgumentException("Forward and Up vectors must be orthogonal");
        }
        
        Forward = Forward.Normalized();
        Up = Up.Normalized();
        _right = Vector3.Cross(Up, Forward).Normalized();
        
        _fovRadians = Fov * MathF.PI / 180;
        
        _screenWidth = 2 * (float)Math.Tan(_fovRadians / 2) * ScreenDistance;
        _resolutionPerUnit = ResolutionWidth / _screenWidth;
        _screenHeight = ResolutionHeight / _resolutionPerUnit;
        
        _screenCenter = new Vector2(_screenWidth / 2, _screenHeight / 2);
        _samples = Sampler.GetSamples(SamplesPerPixel);
        _viewTransformation = new Matrix3(_right, Up, Forward);

        _isInitialized = true;
    }
    
    private Ray[] GenerateRaysInPixel(Vector2 pixel)
    {
        Ray[] rays = new Ray[SamplesPerPixel];
        Vector3 pixelPos = (pixel / _resolutionPerUnit - _screenCenter).To3d();
        int i = 0;
        
        foreach (var sample in _samples)
        {
            Vector3 rayOffset = (sample / _resolutionPerUnit).To3d();
            Vector3 rayOrigin = pixelPos + rayOffset + Forward * ScreenDistance;
            Vector3 rayDirection = rayOrigin * _viewTransformation;
            
            rays[i++] = new Ray(Position, rayDirection);
        }
        
        return rays;
    }
}