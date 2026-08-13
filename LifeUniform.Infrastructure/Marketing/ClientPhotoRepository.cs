using LifeUniform.Domain.Marketing;
using LifeUniform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeUniform.Infrastructure.Marketing;

public class ClientPhotoRepository : IClientPhotoRepository
{
    private readonly ApplicationDbContext _db;

    public ClientPhotoRepository(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<ClientPhoto>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await _db.ClientPhotos
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClientPhoto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _db.ClientPhotos
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClientPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _db.ClientPhotos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpsertAsync(ClientPhoto photo, CancellationToken cancellationToken)
    {
        var existing = await _db.ClientPhotos.FirstOrDefaultAsync(x => x.Id == photo.Id, cancellationToken);
        if (existing is null)
        {
            if (photo.Id == Guid.Empty)
                photo.Id = Guid.NewGuid();
            _db.ClientPhotos.Add(photo);
        }
        else
        {
            existing.ImageUrl = photo.ImageUrl;
            existing.Title = photo.Title;
            existing.ReviewText = photo.ReviewText;
            existing.Rating = photo.Rating;
            existing.SortOrder = photo.SortOrder;
            existing.IsActive = photo.IsActive;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var photo = await _db.ClientPhotos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Client photo not found: {id}");
        _db.ClientPhotos.Remove(photo);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
