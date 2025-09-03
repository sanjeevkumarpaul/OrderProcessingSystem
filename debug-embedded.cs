using System;
using System.IO;
using System.Reflection;
using System.Linq;

class Program
{
    static void Main()
    {
        // Load the Core assembly by path
        var corePath = Path.Combine(Directory.GetCurrentDirectory(), "OrderProcessingSystem.Core", "bin", "Debug", "net9.0", "OrderProcessingSystem.Core.dll");
        
        if (!File.Exists(corePath))
        {
            Console.WriteLine($"Core assembly not found at: {corePath}");
            Console.WriteLine("Trying alternative paths...");
            
            var alternatives = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "OrderProcessingSystem.Core.dll"),
                Path.Combine(Directory.GetCurrentDirectory(), "OrderProcessingSystem.Core.dll"),
                "OrderProcessingSystem.Core.dll"
            };
            
            foreach (var alt in alternatives)
            {
                Console.WriteLine($"Checking: {alt} - {File.Exists(alt)}");
                if (File.Exists(alt))
                {
                    corePath = alt;
                    break;
                }
            }
            
            if (!File.Exists(corePath))
            {
                Console.WriteLine("Could not find Core assembly");
                return;
            }
        }
        
        Console.WriteLine($"Loading assembly from: {corePath}");
        
        try
        {
            var asm = Assembly.LoadFrom(corePath);
            var names = asm.GetManifestResourceNames();
            
            Console.WriteLine($"Found {names.Length} embedded resources:");
            foreach (var name in names)
            {
                Console.WriteLine($"  - {name}");
            }
            
            var gridResource = names.FirstOrDefault(n => n.Contains("grid-columns.json"));
            if (gridResource != null)
            {
                Console.WriteLine($"\nReading grid resource: {gridResource}");
                using var stream = asm.GetManifestResourceStream(gridResource);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var content = reader.ReadToEnd();
                    Console.WriteLine("Content:");
                    Console.WriteLine(content.Substring(0, Math.Min(500, content.Length)));
                }
                else
                {
                    Console.WriteLine("Stream is null");
                }
            }
            else
            {
                Console.WriteLine("No grid-columns.json resource found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
