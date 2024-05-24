using CommandLine;
using OpenTK.Mathematics;
using rt004.Optics.Renderers;
using rt004.SceneDefinition;
using rt004.Utils;
using Util;

namespace rt004;

internal class Program
{
  static void Main(string[] args)
  {
    try
    {
      Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
      {
        CheckArguments(o);
        Configuration config = Configuration.Load(o.ConfigFile, o.JsonTrace);
        Console.WriteLine("Scene initialized.");
        
        Scene scene = config.Scene;
        IRenderer renderer = config.Renderer;
        
        if (o.ResolutionSet) scene.Camera.SetResolution(o.Width, o.Height);
        if (o.SamplesSet) scene.Camera.SetSamples(o.SamplesPerPixel);

        FloatImage image = renderer.Render(scene, config.Brdf);
        image.SavePFM(o.OutputFile);
        
        Console.WriteLine($"HDR image '{o.OutputFile}' is finished.");
        Console.WriteLine(config.Author);
      });
    }
    catch (ArgumentException e)
    {
      Console.WriteLine(e.Message);
    }
  }

  private static void CheckArguments(Options options)
  {
    if (!File.Exists(options.ConfigFile))
    {
      throw new ArgumentException("Configuration file could not be found.");
    }

    string outputDirectory = Path.GetDirectoryName(options.OutputFile)!;
    
    if (!Directory.Exists(outputDirectory))
    {
      throw new ArgumentException("Output file could not be found.");
    }
  }
}
