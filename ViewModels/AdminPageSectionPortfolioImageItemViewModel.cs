namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionPortfolioImageItemViewModel
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }

    public int SortOrder { get; init; }
}
