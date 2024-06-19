using CommandLine;

namespace rt004.Utils;

public class Options
{
    [Option('w', "width", Required = false, HelpText = "Width of the image")]
    public int Width { get; set; } = 0;

    [Option('h', "height", Required = false, HelpText = "Height of the image")]
    public int Height { get; set; } = 0;

    [Option('s', "samples", Required = false, HelpText = "Amount of sample rays per pixel")]
    public int SamplesPerPixel { get; set; } = 0;
    
    [Option('o', "output", Required = false, Default = "./Outputs/demo.pfm", HelpText = "Output file name")]
    public string OutputFile { get; set; }
    
    [Option('c', "config", Required = false, Default = "./Properties/config.json", HelpText = "Configuration file name")]
    public string ConfigFile { get; set; }
    
    [Option('t', "trace", Required = false, Default = false, HelpText = "Trace JSON deserialization")]
    public bool JsonTrace { get; set; }
    
    [Option('p', "parallel", Required = false, HelpText = "Turn on parallel ray processing")]
    public bool DontUseParallel { get; set; }
    
    public bool ResolutionSet => Width > 0 && Height > 0;
    public bool SamplesSet => SamplesPerPixel > 0;
}