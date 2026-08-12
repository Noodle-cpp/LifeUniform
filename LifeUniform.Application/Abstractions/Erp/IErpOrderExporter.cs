namespace LifeUniform.Application.Abstractions.Erp;

public interface IErpOrderExporter
{
    Task ExportOrderAsync(Guid orderId, CancellationToken cancellationToken);
}
