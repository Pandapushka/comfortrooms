namespace ComfortRooms.ViewModels;

public sealed class AdminPageImageItemViewModel
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }

    public int SortOrder { get; init; }
}
