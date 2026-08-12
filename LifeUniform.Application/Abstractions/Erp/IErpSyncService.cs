namespace LifeUniform.Application.Abstractions.Erp;

public interface IErpSyncService
{
    Task SyncAsync(CancellationToken cancellationToken);
}
