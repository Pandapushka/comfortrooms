namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionTestimonialItemViewModel
{
    public int Id { get; init; }

    public required string Title { get; init; }

    public required string Text { get; init; }

    public string? Author { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }

    public int SortOrder { get; init; }

    public bool IsPublished { get; init; }
}
