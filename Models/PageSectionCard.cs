namespace ComfortRooms.Models;

public sealed class PageSectionCard
{
    public int Id { get; set; }

    public int PageSectionId { get; set; }

    public PageSection? PageSection { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsLink { get; set; }

    public string? Url { get; set; }

    public int SortOrder { get; set; }
}
