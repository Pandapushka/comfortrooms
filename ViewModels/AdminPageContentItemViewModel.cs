namespace ComfortRooms.ViewModels;

public sealed class AdminPageContentItemViewModel
{
    public required AdminPageContentBlockViewModel TextBlock { get; init; }

    public AdminPageContentBlockViewModel? ColorBlock { get; init; }

    public AdminPageContentBlockViewModel? StyleBlock { get; init; }
}
