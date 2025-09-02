using System;
using System.Reflection;
using OrderProcessingSystem.Contracts.Interfaces;

Console.WriteLine("Checking ISqlProvider...");

try 
{
    var type = typeof(ISqlProvider);
    Console.WriteLine($"ISqlProvider type: {type.FullName}");
    Console.WriteLine($"Assembly: {type.Assembly.FullName}");
    Console.WriteLine("Success!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
