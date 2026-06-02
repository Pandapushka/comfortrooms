namespace ComfortRooms.ViewModels;

public sealed class PageSectionViewModel
{
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

    public IReadOnlyList<PageSectionButtonViewModel> Buttons { get; init; } = [];

    public IReadOnlyList<PageSectionCardViewModel> Cards { get; init; } = [];

    public IReadOnlyList<PageSectionTestimonialViewModel> Testimonials { get; init; } = [];

    public IReadOnlyList<PageSectionPortfolioImageViewModel> PortfolioImages { get; init; } = [];
}
