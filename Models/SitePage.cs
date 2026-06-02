namespace ComfortRooms.Models;

public sealed class SitePage
{
    public int Id { get; set; }

    public required string Slug { get; set; }

    public required string Title { get; set; }

    public int SortOrder { get; set; }

    public List<PageImage> Images { get; set; } = [];

    public List<PageContentBlock> ContentBlocks { get; set; } = [];

    public List<PageSection> Sections { get; set; } = [];
}
