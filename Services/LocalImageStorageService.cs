namespace ComfortRooms.Services;

public sealed class LocalImageStorageService(IWebHostEnvironment environment) : IImageStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".gif"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    public async Task<string> SaveImageAsync(IFormFile file, string pageSlug, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException("Файл пустой.");
        }

        if (file.Length > 8 * 1024 * 1024)
        {
            throw new InvalidOperationException("Размер изображения не должен превышать 8 МБ.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Разрешены только изображения JPG, PNG, WEBP или GIF.");
        }

        var safeSlug = string.Concat(pageSlug.Select(character => char.IsLetterOrDigit(character) || character == '-' ? character : '-'));
        var uploadRoot = Path.Combine(environment.WebRootPath, "uploads", safeSlug);
        Directory.CreateDirectory(uploadRoot);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var filePath = Path.Combine(uploadRoot, fileName);

        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{safeSlug}/{fileName}";
    }
}
