namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionCardItemViewModel
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public bool IsLink { get; init; }

    public string? Url { get; init; }

    public int SortOrder { get; init; }
}
