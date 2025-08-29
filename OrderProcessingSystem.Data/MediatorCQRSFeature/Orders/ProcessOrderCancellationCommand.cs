using MediatR;
using OrderProcessingSystem.Events.Models;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.Orders
{
    public class ProcessOrderCancellationCommand : IRequest<bool>
    {
        public OrderCancellationSchema OrderCancellation { get; set; }

        public ProcessOrderCancellationCommand(OrderCancellationSchema orderCancellation)
        {
            OrderCancellation = orderCancellation;
        }
    }
}
