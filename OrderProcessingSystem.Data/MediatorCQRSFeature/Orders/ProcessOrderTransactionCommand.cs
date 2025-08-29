using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Events.Models;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.Orders;

public record ProcessOrderTransactionCommand(OrderTransactionSchema OrderTransaction) : IRequest<Order>;
