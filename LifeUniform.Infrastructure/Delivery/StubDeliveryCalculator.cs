using LifeUniform.Application.Abstractions.Delivery;
using LifeUniform.Domain.Orders;

namespace LifeUniform.Infrastructure.Delivery;

public class StubDeliveryCalculator : IDeliveryCalculator
{
    public decimal CalculateFee(DeliveryMethod method, decimal itemsTotal) =>
        method switch
        {
            DeliveryMethod.Cdek => 350m,
            DeliveryMethod.RussianPost => 290m,
            _ => 350m
        };
}
