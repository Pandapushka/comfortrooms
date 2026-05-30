namespace ComfortRooms.Services;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(IFormFile file, string pageSlug, CancellationToken cancellationToken);
}
