namespace LifeUniform.Application.Abstractions.Erp;

public interface IErpCatalogImporter
{
    Task ImportAsync(CancellationToken cancellationToken);
}
