using System.IO;
using System.Reflection;
using System.Text.Json;

namespace OrderProcessingSystem.Core.Metadata;

public static class GridMetadataProvider
{
    public static System.Text.Json.JsonDocument? ReadMetadata()
    {
        // Use the assembly that contains this provider to reliably find embedded resources
        var asm = typeof(GridMetadataProvider).Assembly;
        var names = asm.GetManifestResourceNames();
        
        // Look for grid-columns.json in JsonConfigurations folder first
        var candidate = names.FirstOrDefault(n => n.Contains("JsonConfigurations") && n.EndsWith("grid-columns.json", StringComparison.OrdinalIgnoreCase));
                        
        Console.WriteLine($"Available embedded resources: {string.Join(", ", names)}");
        Console.WriteLine($"Selected candidate: {candidate}");
        Console.WriteLine($"Looking for resource containing 'JsonConfigurations' and ending with 'grid-columns.json'");
        
        if (candidate == null)
        {
            // no embedded resource found - attempt filesystem fallbacks (helps in dev when resource isn't embedded)
            var asmDir = Path.GetDirectoryName(asm.Location) ?? AppContext.BaseDirectory;
            var candidates = new[] {
                Path.Combine(asmDir, "JsonConfigurations", "grid-columns.json"),
                Path.Combine(AppContext.BaseDirectory, "JsonConfigurations", "grid-columns.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "OrderProcessingSystem.Core", "JsonConfigurations", "grid-columns.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "JsonConfigurations", "grid-columns.json")
            };
            
            Console.WriteLine($"Trying filesystem fallbacks...");
            foreach (var p in candidates)
            {
                Console.WriteLine($"Checking path: {p} - Exists: {File.Exists(p)}");
                if (File.Exists(p))
                {
                    try
                    {
                        Console.WriteLine($"Successfully reading from filesystem: {p}");
                        using var fs = File.OpenRead(p);
                        return System.Text.Json.JsonDocument.Parse(fs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading from {p}: {ex.Message}");
                    }
                }
            }
            return null;
        }
        
        using var s = asm.GetManifestResourceStream(candidate);
        Console.WriteLine($"Reading from embedded resource: {candidate}");
        if (s == null) 
        {
            Console.WriteLine("Embedded resource stream is null");
            return null;
        }
        
        try
        {
            return System.Text.Json.JsonDocument.Parse(s);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing embedded resource: {ex.Message}");
            return null;
        }
    }
}
