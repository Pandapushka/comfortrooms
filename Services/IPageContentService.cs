using ComfortRooms.ViewModels;

namespace ComfortRooms.Services;

public interface IPageContentService
{
    Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, string>> GetTextBlocksAsync(string pageSlug, CancellationToken cancellationToken);

    Task<IReadOnlyList<GalleryImageViewModel>> GetGalleryImagesAsync(string pageSlug, CancellationToken cancellationToken);

    Task<IReadOnlyList<GalleryImageViewModel>> GetPageImagesAsync(string pageSlug, CancellationToken cancellationToken);

    Task<IReadOnlyList<HomeTestimonialViewModel>> GetHomeTestimonialsAsync(CancellationToken cancellationToken);
}
