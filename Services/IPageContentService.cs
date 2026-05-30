using ComfortRooms.ViewModels;

namespace ComfortRooms.Services;

public interface IPageContentService
{
    Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken);
}
