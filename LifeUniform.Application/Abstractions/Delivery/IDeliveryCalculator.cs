using LifeUniform.Domain.Orders;

namespace LifeUniform.Application.Abstractions.Delivery;

public interface IDeliveryCalculator
{
    decimal CalculateFee(DeliveryMethod method, decimal itemsTotal);
}
