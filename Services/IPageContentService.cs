using ComfortRooms.ViewModels;

namespace ComfortRooms.Services;

public interface IPageContentService
{
    Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, string>> GetTextBlocksAsync(string pageSlug, CancellationToken cancellationToken);
}
