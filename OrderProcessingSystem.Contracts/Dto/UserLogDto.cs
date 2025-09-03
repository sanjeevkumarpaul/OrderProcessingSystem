namespace OrderProcessingSystem.Contracts.Dto;

public class LoginEventRequest
{
    public string EventType { get; set; } = string.Empty; // MANAGER, ADMIN, USER
}

public class LoginEventResponse
{
    public int Id { get; set; }
    public DateTime EventDate { get; set; }
    public string Event { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UserLogDto
{
    public int Id { get; set; }
    public DateTime EventDate { get; set; }
    public string Event { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class UserLogListResponse
{
    public List<UserLogDto> UserLogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
