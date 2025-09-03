using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.UserLogs;

public record CreateUserLogCommand(UserLog UserLog) : IRequest<UserLog>;
