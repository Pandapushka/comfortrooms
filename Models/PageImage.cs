namespace ComfortRooms.Models;

public sealed class PageImage
{
    public int Id { get; set; }

    public int SitePageId { get; set; }

    public SitePage? SitePage { get; set; }

    public required string Title { get; set; }

    public required string ImageUrl { get; set; }

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
