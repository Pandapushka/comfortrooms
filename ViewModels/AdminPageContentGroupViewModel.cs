namespace ComfortRooms.ViewModels;

public sealed class AdminPageContentGroupViewModel
{
    public required string Title { get; init; }

    public string? Description { get; init; }

    public AdminPageContentBlockViewModel? BackgroundBlock { get; init; }

    public IReadOnlyList<AdminPageContentItemViewModel> Items { get; init; } = [];
}
