using System;
using System.IO;
using System.Reflection;
using System.Linq;

// Copy of the GridMetadataProvider to test
public static class GridMetadataProvider
{
    public static System.Text.Json.JsonDocument? ReadMetadata()
    {
        // Load the Core assembly explicitly 
        var corePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "OrderProcessingSystem.Core", "bin", "Debug", "net9.0", "OrderProcessingSystem.Core.dll");
        var asm = Assembly.LoadFrom(corePath);
        
        var names = asm.GetManifestResourceNames();
        
        // Look for grid-columns.json in JsonConfigurations folder first
        var candidate = names.FirstOrDefault(n => n.Contains("JsonConfigurations") && n.EndsWith("grid-columns.json", StringComparison.OrdinalIgnoreCase));
                        
        Console.WriteLine($"Available embedded resources: {string.Join(", ", names)}");
        Console.WriteLine($"Selected candidate: {candidate}");
        Console.WriteLine($"Looking for resource containing 'JsonConfigurations' and ending with 'grid-columns.json'");
        
        if (candidate == null)
        {
            Console.WriteLine("No candidate found");
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

class Program
{
    static void Main()
    {
        Console.WriteLine("Testing GridMetadataProvider...");
        
        try
        {
            var doc = GridMetadataProvider.ReadMetadata();
            if (doc != null)
            {
                Console.WriteLine("SUCCESS: Grid metadata loaded successfully!");
                
                // Try to access the customers property
                if (doc.RootElement.TryGetProperty("customers", out var customers))
                {
                    Console.WriteLine($"Found {customers.GetArrayLength()} customer columns");
                }
                if (doc.RootElement.TryGetProperty("orders", out var orders))
                {
                    Console.WriteLine($"Found {orders.GetArrayLength()} order columns");
                }
            }
            else
            {
                Console.WriteLine("FAILED: Grid metadata returned null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }
}
