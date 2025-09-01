namespace OrderProcessingSystem.Data.Entities;

public class TransException
{
    public int TransExceptionId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // ORDERCREATION or ORDERCANCELLATION
    public string InputMessage { get; set; } = string.Empty; // JSON message that was passed
    public string Reason { get; set; } = string.Empty; // Reason for being caught at TransException table
    public DateTime RunTime { get; set; } = DateTime.UtcNow; // Date and Time with default constraint
}
