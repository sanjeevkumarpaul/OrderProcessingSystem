using System;
using System.Reflection;
using OrderProcessingSystem.Core.Metadata;

// Quick test to see if embedded resources are working
var result = GridMetadataProvider.ReadMetadata();
if (result != null)
{
    Console.WriteLine("✅ Successfully read grid metadata!");
    Console.WriteLine($"Root element properties: {string.Join(", ", result.RootElement.EnumerateObject().Select(p => p.Name))}");
}
else
{
    Console.WriteLine("❌ Failed to read grid metadata");
}

// Also show available embedded resources
var asm = typeof(GridMetadataProvider).Assembly;
var names = asm.GetManifestResourceNames();
Console.WriteLine($"\nAvailable embedded resources:");
foreach(var name in names)
{
    Console.WriteLine($"  - {name}");
}
