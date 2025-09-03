namespace OrderProcessingSystem.Data.Entities;

public class UserLog
{
    public int Id { get; set; }
    public DateTime EventDate { get; set; }
    public string Event { get; set; } = string.Empty;
    public string EventFlag { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
