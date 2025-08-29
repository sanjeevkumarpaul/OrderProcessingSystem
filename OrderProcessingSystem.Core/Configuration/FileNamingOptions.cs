namespace OrderProcessingSystem.Core.Configuration;

/// <summary>
/// Configuration options for file naming conventions
/// </summary>
public class FileNamingOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "FileNaming";

    /// <summary>
    /// BlobStorage related file naming configuration
    /// </summary>
    public BlobStorageFileNames BlobStorage { get; set; } = new();

    /// <summary>
    /// Solution related file naming configuration
    /// </summary>
    public SolutionFileNames Solution { get; set; } = new();
}

/// <summary>
/// File naming configuration for BlobStorage simulation
/// </summary>
public class BlobStorageFileNames
{
    /// <summary>
    /// File name for order transaction JSON files
    /// </summary>
    public string OrderTransaction { get; set; } = "OrderTransaction.json";

    /// <summary>
    /// File name for order cancellation JSON files
    /// </summary>
    public string OrderCancellation { get; set; } = "OrderCancellation.json";

    /// <summary>
    /// Directory name for BlobStorage simulation
    /// </summary>
    public string DirectoryName { get; set; } = "BlobStorageSimulation";

    /// <summary>
    /// Directory name for sample files
    /// </summary>
    public string SampleDirectory { get; set; } = "_Sample";
}

/// <summary>
/// File naming configuration for solution files
/// </summary>
public class SolutionFileNames
{
    /// <summary>
    /// Main solution file name
    /// </summary>
    public string OrderProcessingSystem { get; set; } = "OrderProcessingSystem.sln";
}
