using MediatR;



namespace LifeUniform.Application.Catalog.Commands;



public class DeleteProductImageCommand : IRequest

{

    public string Slug { get; init; } = string.Empty;

    public Guid ImageId { get; init; }

}



public class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand>

{

    private readonly LifeUniform.Domain.Catalog.ICatalogRepository _catalogRepository;

    private readonly LifeUniform.Application.Abstractions.Caching.ICatalogCacheInvalidator _cacheInvalidator;



    public DeleteProductImageHandler(

        LifeUniform.Domain.Catalog.ICatalogRepository catalogRepository,

        LifeUniform.Application.Abstractions.Caching.ICatalogCacheInvalidator cacheInvalidator)

    {

        _catalogRepository = catalogRepository;

        _cacheInvalidator = cacheInvalidator;

    }



    public async Task Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)

    {

        await _catalogRepository.DeleteProductImageAsync(request.Slug, request.ImageId, cancellationToken);

        _cacheInvalidator.InvalidateCatalog();

    }

}


