namespace ComfortRooms.ViewModels;

public sealed class AdminPageContentViewModel
{
    public required string PageSlug { get; init; }

    public required string PageTitle { get; init; }

    public IReadOnlyList<AdminPageContentBlockViewModel> Blocks { get; init; } = [];

    public IReadOnlyList<AdminPageContentGroupViewModel> Groups { get; init; } = [];
}
