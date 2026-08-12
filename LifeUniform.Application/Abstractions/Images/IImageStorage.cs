namespace LifeUniform.Application.Abstractions.Images;

public sealed record StoredImageResult(string FileNameOriginal, string FileNamePreview, string FileNameWebp);

public interface IImageStorage
{
    Task<StoredImageResult> SaveProductImageAsync(
        Stream content,
        string originalFileName,
        string slugHint,
        CancellationToken cancellationToken);
}
