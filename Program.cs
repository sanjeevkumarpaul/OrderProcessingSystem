using System;
using System.IO;
using System.Reflection;

Console.WriteLine("Testing embedded resources...");

// First, let's build the Core project to ensure it's up to date
Console.WriteLine("Note: Make sure Core project is built with the latest changes");

// Try to load the Core assembly directly
try
{
    var coreDllPath = "/Users/sanjeevkumarpaul/Desktop/Code Practice/OrderProcessingSystem/OrderProcessingSystem.Core/bin/Debug/net9.0/OrderProcessingSystem.Core.dll";
    
    if (File.Exists(coreDllPath))
    {
        Console.WriteLine($"✅ Core DLL found at: {coreDllPath}");
        
        var coreAssembly = Assembly.LoadFrom(coreDllPath);
        var coreResources = coreAssembly.GetManifestResourceNames();
        
        Console.WriteLine($"\nEmbedded Resources in Core assembly ({coreResources.Length} total):");
        foreach (var resource in coreResources)
        {
            Console.WriteLine($"  {resource}");
            
            // Check if this is our grid-columns.json
            if (resource.Contains("grid-columns.json"))
            {
                Console.WriteLine($"    ✅ Found grid-columns.json as: {resource}");
                
                using var stream = coreAssembly.GetManifestResourceStream(resource);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    var content = reader.ReadToEnd();
                    Console.WriteLine($"\nContent preview (first 200 chars):");
                    Console.WriteLine(content.Length > 200 ? content.Substring(0, 200) + "..." : content);
                }
                else
                {
                    Console.WriteLine("    ❌ Could not read stream");
                }
            }
        }
        
        if (coreResources.Length == 0)
        {
            Console.WriteLine("  ❌ No embedded resources found in Core assembly");
        }
    }
    else
    {
        Console.WriteLine($"❌ Core DLL not found at: {coreDllPath}");
        Console.WriteLine("Please build the Core project first");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error loading Core assembly: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
