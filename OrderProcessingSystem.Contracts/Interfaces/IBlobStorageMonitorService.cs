using Microsoft.Extensions.Hosting;
using OrderProcessingSystem.Core.Enums;

namespace OrderProcessingSystem.Contracts.Interfaces
{
    public interface IBlobStorageMonitorService : IHostedService
    {
        /// <summary>
        /// Queues a task to process a specific file in the monitored folder
        /// </summary>
        /// <param name="fileName">Name of the file to process</param>
        /// <param name="queue">Which queue to use for processing</param>
        /// <returns>Task representing the queuing operation</returns>
        Task QueueFileProcessingTask(string fileName, FileProcessingQueue queue);
    }
}
