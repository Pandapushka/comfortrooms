namespace ComfortRooms.Models;

public sealed class HomeTestimonial
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Text { get; set; }

    public string? Author { get; set; }

    public required string ImageUrl { get; set; }

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPublished { get; set; } = true;
}
