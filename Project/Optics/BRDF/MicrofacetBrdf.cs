using System.Numerics;
using System.Reflection;
using System.Xml.Schema;
using Newtonsoft.Json;
using OpenTK;
using rt004.Optics.LightSources;
using rt004.SceneDefinition;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace rt004.Optics.BRDF;

// Most of the equations are taken from this paper: https://www.cs.cornell.edu/~srm/publications/EGSR07-btdf.pdf
public class MicrofacetBrdf : Brdf
{
    private const float EPSILON = 1e-7f;
    
    // Stores any kind of variable that has to be used in the calculations at least once
    private struct Arguments
    {
        public enum Mode { Reflection, Transmission }
        public Intersection Intersection;
        
        public Vector3 ViewDirection;
        public Vector3 LightDirection;
        public Vector3 HalfVector = Vector3.Zero;
        public Vector3 Normal = Vector3.Zero;
        
        public float nhCosine;
        public float nlCosine;
        public float nvCosine;

        public float vhCosine;
        public float lhCosine;

        public float Roughness => Intersection.InnerMaterial.Roughness;

        public Arguments(Vector3 lightDirection, Intersection point, Mode mode)
        {
            Intersection = point;
            ViewDirection = point.ViewDirection;
            LightDirection = lightDirection;
            Normal = point.SurfaceNormal;

            if (mode == Mode.Reflection)
            {
                HalfVector = (ViewDirection + LightDirection).Normalized();
            }
            
            if (mode == Mode.Transmission)
            {
                float n1 = point.OuterMaterial.RefractiveIndex;
                float n2 = point.InnerMaterial.RefractiveIndex;
                
                HalfVector = (-n1 * point.ViewDirection - n2 * lightDirection).Normalized();
            }

            nhCosine = Vector3.Dot(Normal, HalfVector);
            nlCosine = Vector3.Dot(Normal, LightDirection);
            nvCosine = Vector3.Dot(Normal, ViewDirection);

            vhCosine = Vector3.Dot(ViewDirection, HalfVector);
            lhCosine = Vector3.Dot(LightDirection, HalfVector);
        }
    }

    private delegate float MicrofacetFunc(Arguments args);
    [JsonProperty] public string FresnelFunction { get; init; }
    private MicrofacetFunc? _fresnelFunction;
    
    [JsonProperty] public string DistributionFunction { get; init; }
    private MicrofacetFunc? _distributionTFunction;
    
    [JsonProperty] public string GeometryFunction { get; init; }
    private MicrofacetFunc? _geometryFunction;
    
    private bool _isInitialized = false;

    // Equation 20
    public override float GetReflectance(Vector3 lightDirection, Intersection point)
    {
        if (!_isInitialized) Initialize();
        
        var args = new Arguments(lightDirection, point, Arguments.Mode.Reflection);
        if (args.nlCosine < 0 || args.nvCosine < 0) return 0;
        
        float fresnelTerm = _fresnelFunction!(args);
        float distributionTerm = _distributionTFunction!(args);
        float geometryTerm = _geometryFunction!(args);
        
        float reflectance = (fresnelTerm * distributionTerm * geometryTerm) / (4 * args.nlCosine * args.nvCosine);
        return Math.Clamp(reflectance, 0, 1);
    }

    // Equation 21
    public override float GetTransmittance(Vector3 lightDirection, Intersection point)
    {
        if (!_isInitialized) Initialize();

        var args = new Arguments(lightDirection, point, Arguments.Mode.Transmission);

        float n1 = point.OuterMaterial.RefractiveIndex;
        float n2 = point.InnerMaterial.RefractiveIndex;

        float fresnelTerm = 1 - _fresnelFunction!(args);
        float distributionTerm = _distributionTFunction!(args);
        float geometryTerm = _geometryFunction!(args);
        
        float cosineFraction = MathF.Abs(args.lhCosine * args.vhCosine / args.nlCosine * args.nhCosine);
        float microfacetTerms = fresnelTerm * distributionTerm * geometryTerm;
        float denomTerm = n1 * args.vhCosine + n2 * args.lhCosine;

        float transmittance = n2 * n2 * cosineFraction * microfacetTerms / (denomTerm * denomTerm);
        
        return Math.Clamp(transmittance, 0, 1);
    }

    private float SchlicksFresnel(Arguments args)
    {
        float n1 = args.Intersection.OuterMaterial.RefractiveIndex;
        float n2 = args.Intersection.InnerMaterial.RefractiveIndex;
        
        float perpFresnel = MathF.Pow((n2 - n1) / (n2 + n1), 2);

        float fresnelTerm = perpFresnel + (1 - perpFresnel) * MathF.Pow(1 - args.lhCosine, 5);
        return fresnelTerm;
    }
    
    private float BlinnPhongDistribution(Arguments args)
    {
        if (args.nhCosine <= 0) return 0;
        
        float distributionTerm = (args.Roughness + 2) * MathF.Pow(args.nhCosine, args.Roughness) / (2 * MathF.PI);
        return distributionTerm;
    }

    private float TrowbridgeReitzDistribution(Arguments args)
    {
        if (args.nhCosine <= 0) return 0;

        float rSquared = args.Roughness * args.Roughness;
        float nhSquared = args.nhCosine * args.nhCosine;
        float denomTerm = nhSquared * (rSquared - 1) + 1;
        
        float distributionTerm = rSquared / (MathF.PI * denomTerm * denomTerm);
        return distributionTerm;
    }

    private float GGXDistribution(Arguments args)
    {
        if (args.nhCosine <= 0) return 0;

        float rSquared = args.Roughness * args.Roughness;
        
        float nhCosine2 = args.nhCosine * args.nhCosine;
        float nhCosine4 = nhCosine2 * nhCosine2;
        float nhTangent2 = (1 - nhCosine2) / nhCosine2;             // as tan^2 = sin^2 / cos^2 and sin^2 = 1 - cos^2 

        float denomTerm = rSquared + nhTangent2;
        float distributionTerm = rSquared / (MathF.PI * nhCosine4 * denomTerm * denomTerm);
        return distributionTerm;
    }

    // Equation 27
    private float BeckmannGeometry(Arguments args)
    {
        if (args.vhCosine / args.nvCosine < 0) return 0;
        if (args.lhCosine / args.lhCosine < 0) return 0;

        Func<float, float> smithG1 = dhCosine =>
        {
            float dhCosine2 = args.nvCosine * args.nvCosine;
            float dhTangent2 = (1 - dhCosine2) / dhCosine2;

            float param = 1 / args.Roughness * MathF.Sqrt(dhTangent2);

            if (param >= 1.6) return 1;

            float param2 = param * param;
            float g1Term = (3.535f * param + 2.181f * param2) / (1f + 2.276f * param + 2.577f * param2);
            return g1Term;
        };
        
        return smithG1(args.lhCosine) * smithG1(args.vhCosine);
    }

    // Equations 23 and 34
    private float GGXGeometry(Arguments args)
    {
        if (args.vhCosine / args.nvCosine < 0) return 0;
        if (args.lhCosine / args.lhCosine < 0) return 0;

        Func<float, float> smithG1 = dhCosine =>
        {
            float rSquared = args.Roughness * args.Roughness;
            float dhCosine2 = dhCosine * dhCosine;
            float dhTangent2 = (1 - dhCosine2) / dhCosine2;

            float g1Term = 2 / (1 + MathF.Sqrt(1 + rSquared * dhTangent2));
            return g1Term;
        };

        return smithG1(args.lhCosine) * smithG1(args.vhCosine);
    }

    private void Initialize()
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        
        string fresnelMethodName = FresnelFunction + "Fresnel";
        _fresnelFunction = GetType().
            GetMethod(fresnelMethodName, flags)?
            .CreateDelegate<MicrofacetFunc>(this);
        
        if (_fresnelFunction == null) 
            throw new ArgumentException("Could not find method for the Fresnel term.");
        
        string distributionMethodName = DistributionFunction + "Distribution";
        _distributionTFunction = GetType()
            .GetMethod(distributionMethodName, flags)?
            .CreateDelegate<MicrofacetFunc>(this);
        
        if (_distributionTFunction == null) 
            throw new ArgumentException("Could not find method for the distribution term.");
        
        string geometricMethodName = GeometryFunction + "Geometry";
        _geometryFunction = GetType()
            .GetMethod(geometricMethodName, flags)?
            .CreateDelegate<MicrofacetFunc>(this);
        
        if (_geometryFunction == null) 
            throw new ArgumentException("Could not find method for the geometric term.");
        
        _isInitialized = true;
    }

}