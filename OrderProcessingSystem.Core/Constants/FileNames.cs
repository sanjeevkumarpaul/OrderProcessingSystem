namespace OrderProcessingSystem.Core.Constants;

/// <summary>
/// Contains constant values for file names used throughout the application
/// </summary>
public static class FileNames
{
    /// <summary>
    /// File naming constants for BlobStorage simulation files
    /// </summary>
    public static class BlobStorage
    {
        /// <summary>
        /// File name for order transaction JSON files
        /// </summary>
        public const string OrderTransaction = "OrderTransaction.json";

        /// <summary>
        /// File name for order cancellation JSON files
        /// </summary>
        public const string OrderCancellation = "OrderCancellation.json";

        /// <summary>
        /// Directory name for BlobStorage simulation
        /// </summary>
        public const string DirectoryName = "BlobStorageSimulation";

        /// <summary>
        /// Directory name for sample files
        /// </summary>
        public const string SampleDirectory = "_Sample";
    }

    /// <summary>
    /// File naming constants for configuration files
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Application settings file name
        /// </summary>
        public const string AppSettings = "appsettings.json";

        /// <summary>
        /// Development settings file name
        /// </summary>
        public const string AppSettingsDevelopment = "appsettings.Development.json";

        /// <summary>
        /// Local settings file name
        /// </summary>
        public const string AppSettingsLocal = "appsettings.Local.json";
    }

    /// <summary>
    /// File naming constants for solution files
    /// </summary>
    public static class Solution
    {
        /// <summary>
        /// Main solution file name
        /// </summary>
        public const string OrderProcessingSystem = "OrderProcessingSystem.sln";
    }
}
