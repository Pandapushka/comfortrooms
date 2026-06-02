namespace ComfortRooms.Models;

public sealed class PageSectionButton
{
    public int Id { get; set; }

    public int PageSectionId { get; set; }

    public PageSection? PageSection { get; set; }

    public required string Text { get; set; }

    public required string Url { get; set; }

    public required string StyleClass { get; set; }

    public int SortOrder { get; set; }
}
