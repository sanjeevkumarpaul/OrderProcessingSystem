using MediatR;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.MediatorCQRS.UserLogs;

namespace OrderProcessingSystem.Infrastructure.Services;

public class UserLogService : IUserLogService
{
    private readonly IMediator _mediator;
    private readonly Random _random = new();

    // Sample data for generating random user information
    private readonly string[] _companyDomains = {
        "acmecorp.com", "globexltd.com", "stellartech.com", "pinnaclegroup.com", "nexusinnovations.com",
        "vanguardenterprises.com", "meridiantech.com", "apexsolutions.com", "titanindustries.com", "omegacorp.com",
        "deltatech.com", "infinitelogistics.com", "primeventures.com", "elitemanufacturing.com", "summitconsulting.com",
        "phoenixtech.com", "quantumcorp.com", "fusionenterprises.com", "catalystgroup.com", "dynamicsolutions.com"
    };

    private readonly string[] _firstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth",
        "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah", "Christopher", "Karen",
        "Charles", "Nancy", "Daniel", "Matthew", "Lisa", "Betty", "Helen", "Sandra", "Anthony", "Mark",
        "Donald", "Steven", "Donna", "Carol", "Paul", "Ruth", "Andrew", "Sharon", "Joshua", "Michelle",
        "Kenneth", "Laura", "Kevin", "Kimberly", "Brian", "Deborah", "George", "Edward", "Dorothy", "Amy"
    };

    private readonly string[] _lastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
    };

    public UserLogService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<LoginEventResponse> LogLoginEventAsync(LoginEventRequest request)
    {
        try
        {
            // Validate event type
            var validEvents = new[] { "MANAGER", "ADMIN", "USER" };
            if (!validEvents.Contains(request.EventType.ToUpper()))
            {
                return new LoginEventResponse
                {
                    Success = false,
                    Message = $"Invalid event type. Must be one of: {string.Join(", ", validEvents)}"
                };
            }

            // Generate random user information
            var firstName = _firstNames[_random.Next(_firstNames.Length)];
            var lastName = _lastNames[_random.Next(_lastNames.Length)];
            var domain = _companyDomains[_random.Next(_companyDomains.Length)];
            var userId = $"{firstName.ToLower()}.{lastName.ToLower()}@{domain}";
            var userName = $"{firstName} {lastName}";

            // Create UserLog entity
            var userLog = new UserLog
            {
                EventDate = DateTime.UtcNow,
                Event = request.EventType.ToUpper(),
                UserId = userId,
                UserName = userName
            };

            // Save to database using MediatR
            var savedUserLog = await _mediator.Send(new CreateUserLogCommand(userLog));

            return new LoginEventResponse
            {
                Id = savedUserLog.Id,
                EventDate = savedUserLog.EventDate,
                Event = savedUserLog.Event,
                UserId = savedUserLog.UserId,
                UserName = savedUserLog.UserName,
                Success = true,
                Message = $"Login event logged successfully for {savedUserLog.Event} user"
            };
        }
        catch (Exception ex)
        {
            return new LoginEventResponse
            {
                Success = false,
                Message = $"Error logging event: {ex.Message}"
            };
        }
    }

    public async Task<UserLogListResponse> GetUserLogsAsync(int pageNumber = 1, int pageSize = 50, string? eventType = null)
    {
        try
        {
            var query = new GetUserLogsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                EventType = eventType
            };

            var result = await _mediator.Send(query);
            var userLogDtos = result.logs;
            var totalCount = result.totalCount;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new UserLogListResponse
            {
                UserLogs = userLogDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception)
        {
            return new UserLogListResponse
            {
                UserLogs = new List<UserLogDto>(),
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 0
            };
        }
    }
}
