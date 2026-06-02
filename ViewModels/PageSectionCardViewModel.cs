namespace ComfortRooms.ViewModels;

public sealed class PageSectionCardViewModel
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public bool IsLink { get; init; }

    public string? Url { get; init; }
}
