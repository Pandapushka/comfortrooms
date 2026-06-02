namespace ComfortRooms.Models;

public sealed class PageSectionPortfolioImage
{
    public int Id { get; set; }

    public int PageSectionId { get; set; }

    public PageSection? PageSection { get; set; }

    public required string Title { get; set; }

    public required string ImageUrl { get; set; }

    public string? AltText { get; set; }

    public int SortOrder { get; set; }
}
