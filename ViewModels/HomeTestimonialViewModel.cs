namespace ComfortRooms.ViewModels;

public sealed class HomeTestimonialViewModel
{
    public required string Title { get; init; }

    public required string Text { get; init; }

    public string? Author { get; init; }

    public required string ImageUrl { get; init; }

    public string? AltText { get; init; }
}
