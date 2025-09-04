using System;

namespace OrderProcessingSystem.UI.Shared.UIViewModels;

/// <summary>
/// ViewModel for UserLog display in grid components
/// Used for chunked data loading and display in the UserActivity page
/// </summary>
public class UserLogVM
{
    public int Id { get; set; }
    public DateTime EventDate { get; set; }
    public string Event { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
