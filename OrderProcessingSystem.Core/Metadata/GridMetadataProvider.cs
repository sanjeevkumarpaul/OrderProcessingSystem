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
        // Prefer an exact match if present, otherwise fallback to any resource ending with grid-columns.json
        // var candidate = names.FirstOrDefault(n => n.EndsWith("Metadata.grid-columns.json", StringComparison.OrdinalIgnoreCase))
        //                 ?? names.FirstOrDefault(n => n.EndsWith("grid-columns.json", StringComparison.OrdinalIgnoreCase));
        var candidate = names.FirstOrDefault(n => n.EndsWith("grid-columns.json", StringComparison.OrdinalIgnoreCase));
        if (candidate == null)
        {
            // no embedded resource found - attempt filesystem fallbacks (helps in dev when resource isn't embedded)
            var asmDir = Path.GetDirectoryName(asm.Location) ?? AppContext.BaseDirectory;
            var candidates = new[] {
                Path.Combine(asmDir, "Metadata", "grid-columns.json"),
                Path.Combine(AppContext.BaseDirectory, "Metadata", "grid-columns.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "OrderProcessingSystem.Core", "Metadata", "grid-columns.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Metadata", "grid-columns.json")
            };
            foreach (var p in candidates)
            {
                if (File.Exists(p))
                {
                    try
                    {
                        using var fs = File.OpenRead(p);
                        return System.Text.Json.JsonDocument.Parse(fs);
                    }
                    catch
                    {
                        Console.WriteLine(p);
                    }
                }
            }
            return null;
        }
        using var s = asm.GetManifestResourceStream(candidate);
        Console.WriteLine($"Candidate resource: {candidate}");
        if (s == null) return null;
        try
        {
            return System.Text.Json.JsonDocument.Parse(s);
        }
        catch
        {
            return null;
        }
    }
}
