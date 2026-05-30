namespace ComfortRooms.ViewModels;

public sealed class GalleryImageViewModel
{
    public required string Title { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }
}
