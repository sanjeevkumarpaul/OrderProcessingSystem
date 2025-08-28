namespace OrderProcessingServer.BackgroundTasks;

public class BlobStorageSimulationOptions
{
    public const string SectionName = "BlobStorageSimulation";
    
    public string FolderPath { get; set; } = string.Empty;
    public string MonitoredFileName { get; set; } = "OrderTransaction.json";
    public int PollingIntervalSeconds { get; set; } = 5;
}
