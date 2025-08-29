using Microsoft.Extensions.Logging;

namespace OrderProcessingSystem.Utilities.Helpers;

/// <summary>
/// Utility class for file operations
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Writes content to a file with logging support
    /// </summary>
    /// <param name="filePath">Full path to the file</param>
    /// <param name="content">Content to write to the file</param>
    /// <param name="logger">Logger instance for logging operations</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task WriteFileAsync(string filePath, string content, ILogger? logger = null)
    {
        var fileName = Path.GetFileName(filePath);
        var directory = Path.GetDirectoryName(filePath);
        
        // Create directory if it doesn't exist
        if (!string.IsNullOrEmpty(directory))
        {
            EnsureDirectoryExists(directory, logger);
        }
        
        logger?.LogInformation($"Creating {fileName} at: {filePath}");
        await File.WriteAllTextAsync(filePath, content);
        logger?.LogInformation($"{fileName} created successfully");
    }

    /// <summary>
    /// Writes JSON content to a file with logging support
    /// </summary>
    /// <param name="directoryPath">Directory path where the file should be created</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="jsonContent">JSON content to write</param>
    /// <param name="logger">Logger instance for logging operations</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public static async Task WriteJsonFileAsync(string directoryPath, string fileName, string jsonContent, ILogger? logger = null)
    {
        var filePath = Path.Combine(directoryPath, fileName);
        await WriteFileAsync(filePath, jsonContent, logger);
    }

    /// <summary>
    /// Reads content from a file
    /// </summary>
    /// <param name="filePath">Full path to the file</param>
    /// <returns>File content as string</returns>
    public static async Task<string> ReadFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        
        return await File.ReadAllTextAsync(filePath);
    }

    /// <summary>
    /// Checks if a file exists
    /// </summary>
    /// <param name="filePath">Full path to the file</param>
    /// <returns>True if file exists, false otherwise</returns>
    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// Ensures a directory exists by creating it if it doesn't exist
    /// </summary>
    /// <param name="directoryPath">Path to the directory to ensure exists</param>
    /// <param name="logger">Logger instance for logging operations</param>
    /// <returns>True if directory was created, false if it already existed</returns>
    public static bool EnsureDirectoryExists(string directoryPath, ILogger? logger = null)
    {
        if (Directory.Exists(directoryPath))
        {
            return false;
        }
        
        Directory.CreateDirectory(directoryPath);
        logger?.LogInformation($"Created directory: {directoryPath}");
        return true;
    }

    /// <summary>
    /// Finds the solution root directory by looking for a .sln file
    /// </summary>
    /// <param name="startingDirectory">Directory to start searching from (optional, defaults to current directory)</param>
    /// <param name="solutionFileName">Name of the solution file to look for (optional)</param>
    /// <returns>Path to the solution root directory</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when solution root cannot be found</exception>
    public static string FindSolutionRoot(string? startingDirectory = null, string? solutionFileName = null)
    {
        var currentDir = startingDirectory ?? Directory.GetCurrentDirectory();
        var solutionRoot = currentDir;
        
        // If no specific solution file name provided, look for any .sln file
        bool SolutionExists(string dir) => solutionFileName != null 
            ? File.Exists(Path.Combine(dir, solutionFileName))
            : Directory.GetFiles(dir, "*.sln").Length > 0;
        
        // Keep going up until we find the solution root
        while (solutionRoot != null && !SolutionExists(solutionRoot))
        {
            solutionRoot = Directory.GetParent(solutionRoot)?.FullName;
        }
        
        if (solutionRoot == null)
        {
            throw new DirectoryNotFoundException("Could not find solution root directory");
        }
        
        return solutionRoot;
    }
}
