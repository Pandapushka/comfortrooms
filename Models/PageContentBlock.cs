namespace ComfortRooms.Models;

public sealed class PageContentBlock
{
    public int Id { get; set; }

    public int SitePageId { get; set; }

    public SitePage? SitePage { get; set; }

    public required string Key { get; set; }

    public required string Label { get; set; }

    public required string Value { get; set; }

    public int SortOrder { get; set; }
}
