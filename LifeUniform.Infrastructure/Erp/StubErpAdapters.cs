using LifeUniform.Application.Abstractions.Erp;
using Microsoft.Extensions.Logging;

namespace LifeUniform.Infrastructure.Erp;

public class StubErpCatalogImporter : IErpCatalogImporter
{
    private readonly ILogger<StubErpCatalogImporter> _logger;
    public StubErpCatalogImporter(ILogger<StubErpCatalogImporter> logger) => _logger = logger;

    public Task ImportAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stub 1C catalog import — no-op");
        return Task.CompletedTask;
    }
}

public class StubErpOrderExporter : IErpOrderExporter
{
    private readonly ILogger<StubErpOrderExporter> _logger;
    public StubErpOrderExporter(ILogger<StubErpOrderExporter> logger) => _logger = logger;

    public Task ExportOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stub 1C order export for {OrderId} — no-op", orderId);
        return Task.CompletedTask;
    }
}

public class StubErpSyncService : IErpSyncService
{
    private readonly IErpCatalogImporter _importer;
    private readonly ILogger<StubErpSyncService> _logger;

    public StubErpSyncService(IErpCatalogImporter importer, ILogger<StubErpSyncService> logger)
    {
        _importer = importer;
        _logger = logger;
    }

    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stub 1C sync started");
        await _importer.ImportAsync(cancellationToken);
        _logger.LogInformation("Stub 1C sync finished");
    }
}
