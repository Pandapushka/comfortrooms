namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionItemViewModel
{
    public int Id { get; init; }

    public required string TemplateKey { get; init; }

    public required string LayoutKey { get; init; }

    public string? Eyebrow { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public string? ImageUrl { get; init; }

    public string? ImageAltText { get; init; }

    public required string BackgroundClass { get; init; }

    public required string EyebrowColorClass { get; init; }

    public required string TitleColorClass { get; init; }

    public required string DescriptionColorClass { get; init; }

    public int SortOrder { get; init; }

    public bool IsPublished { get; init; }

    public IReadOnlyList<AdminPageSectionButtonItemViewModel> Buttons { get; init; } = [];

    public IReadOnlyList<AdminPageSectionCardItemViewModel> Cards { get; init; } = [];

    public IReadOnlyList<AdminPageSectionTestimonialItemViewModel> Testimonials { get; init; } = [];

    public IReadOnlyList<AdminPageSectionPortfolioImageItemViewModel> PortfolioImages { get; init; } = [];
}
