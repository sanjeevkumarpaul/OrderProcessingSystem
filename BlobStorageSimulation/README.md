# BlobStorageSimulation

This folder simulates a blob storage container that the Blazor application monitors for order transaction files.

## How it works

1. The `BlobStorageMonitorService` background service runs continuously while the Blazor app is running
2. It monitors this folder for files named `OrderTransaction.json`
3. When a file is created or modified, the service processes it automatically
4. The service uses both file system watching and periodic polling for reliability

## Configuration

The monitoring behavior is configured in `appsettings.json`:

```json
{
  "BlobStorageSimulation": {
    "FolderPath": "../BlobStorageSimulation",
    "MonitoredFileName": "OrderTransaction.json",
    "PollingIntervalSeconds": 5
  }
}
```

## Testing

1. Start the Blazor application
2. Copy `sample_OrderTransaction.json` to `OrderTransaction.json` in this folder
3. Check the application logs to see the file being processed
4. Modify the JSON file content to test change detection

## Expected JSON Format

```json
{
  "orderId": "ORDER-2025-001",
  "transactionType": "CREATE|UPDATE|DELETE",
  "timestamp": "2025-08-28T10:30:00Z",
  "customerName": "John Doe",
  "items": [...],
  "orderTotal": 1059.97,
  "status": "PENDING|PROCESSING|COMPLETED|CANCELLED"
}
```

## File Processing

- Files are processed immediately when detected
- The service validates JSON format before processing
- Processed files can be archived (currently commented out)
- Errors are logged but don't stop the monitoring service
