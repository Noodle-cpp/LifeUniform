namespace LifeUniform.Domain.Marketing;

public class ClientPhoto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ReviewText { get; set; }
    public int Rating { get; set; } = 5;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public interface IClientPhotoRepository
{
    Task<IReadOnlyList<ClientPhoto>> GetActiveAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ClientPhoto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClientPhoto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpsertAsync(ClientPhoto photo, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
