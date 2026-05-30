namespace ComfortRooms.ViewModels;

public sealed class AdminPageImagesViewModel
{
    public required string PageSlug { get; init; }

    public required string PageTitle { get; init; }

    public IReadOnlyList<AdminPageImageItemViewModel> Images { get; init; } = [];
}
