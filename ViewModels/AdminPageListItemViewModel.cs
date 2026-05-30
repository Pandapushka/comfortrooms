namespace ComfortRooms.ViewModels;

public sealed class AdminPageListItemViewModel
{
    public required string Slug { get; init; }

    public required string Title { get; init; }

    public int ImagesCount { get; init; }
}
