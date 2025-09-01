using System.ComponentModel.DataAnnotations;

namespace OrderProcessingServer.Shared.UIViewModels;

/// <summary>
/// ViewModel for displaying TransException records in the UI
/// </summary>
public class TransExceptionVM
{
    public int TransExceptionId { get; set; }
    
    [Display(Name = "Transaction Type")]
    public string TransactionType { get; set; } = string.Empty;
    
    [Display(Name = "Input Message")]
    public string InputMessage { get; set; } = string.Empty;
    
    [Display(Name = "Exception Reason")]
    public string Reason { get; set; } = string.Empty;
    
    [Display(Name = "Occurred At")]
    public DateTime RunTime { get; set; }
    
    /// <summary>
    /// Formatted display of the RunTime for better readability
    /// </summary>
    public string FormattedRunTime => RunTime.ToString("yyyy-MM-dd HH:mm:ss");
    
    /// <summary>
    /// Truncated version of InputMessage for grid display
    /// </summary>
    public string TruncatedMessage => InputMessage.Length > 100 ? 
        InputMessage.Substring(0, 100) + "..." : InputMessage;
    
    /// <summary>
    /// Truncated version of Reason for grid display
    /// </summary>
    public string TruncatedReason => Reason.Length > 80 ? 
        Reason.Substring(0, 80) + "..." : Reason;
    
    /// <summary>
    /// User-friendly display of transaction type
    /// </summary>
    public string TransactionTypeDisplay => TransactionType switch
    {
        "ORDERCREATION" => "Order Creation",
        "ORDERCANCELLATION" => "Order Cancellation",
        _ => TransactionType
    };
    
    /// <summary>
    /// CSS class for transaction type badge
    /// </summary>
    public string TransactionTypeBadgeClass => TransactionType switch
    {
        "ORDERCREATION" => "badge bg-success",
        "ORDERCANCELLATION" => "badge bg-danger", 
        _ => "badge bg-secondary"
    };
}
