namespace ComfortRooms.ViewModels;

public sealed class PageSectionPortfolioImageViewModel
{
    public required string Title { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }
}
