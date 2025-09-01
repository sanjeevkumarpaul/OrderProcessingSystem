namespace OrderProcessingSystem.Contracts.Dto;

public class TransExceptionDto
{
    public int TransExceptionId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string InputMessage { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime RunTime { get; set; }
}
