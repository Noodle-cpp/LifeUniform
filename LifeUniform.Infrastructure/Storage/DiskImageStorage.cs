using LifeUniform.Application.Abstractions.Images;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LifeUniform.Infrastructure.Storage;

/// <summary>
/// Dev disk storage under wwwroot/uploads/products.
/// Writes from memory buffer to avoid file-lock races on Windows static files.
/// </summary>
public class DiskImageStorage : IImageStorage
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<DiskImageStorage> _logger;

    public DiskImageStorage(IHostEnvironment env, ILogger<DiskImageStorage> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<StoredImageResult> SaveProductImageAsync(
        Stream content,
        string originalFileName,
        string slugHint,
        CancellationToken cancellationToken)
    {
        var uploadsRoot = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "products");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".img";

        var safeSlug = string.Join("-", (string.IsNullOrWhiteSpace(slugHint) ? "product" : slugHint.Trim())
            .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        if (string.IsNullOrWhiteSpace(safeSlug))
            safeSlug = "product";

        var id = Guid.NewGuid().ToString("N")[..8];
        var fileName = $"{safeSlug}-{id}{ext}";
        var path = Path.Combine(uploadsRoot, fileName);

        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        await using (var fs = new FileStream(
                         path,
                         FileMode.CreateNew,
                         FileAccess.Write,
                         FileShare.Read,
                         bufferSize: 64 * 1024,
                         options: FileOptions.Asynchronous | FileOptions.SequentialScan))
        {
            await fs.WriteAsync(bytes, cancellationToken);
            await fs.FlushAsync(cancellationToken);
        }

        _logger.LogInformation("Saved product image {FileName}", fileName);
        // Same file used as original/preview until a real resize pipeline exists.
        return new StoredImageResult(fileName, fileName, fileName);
    }
}
