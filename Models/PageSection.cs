namespace ComfortRooms.Models;

public sealed class PageSection
{
    public int Id { get; set; }

    public int SitePageId { get; set; }

    public SitePage? SitePage { get; set; }

    public required string TemplateKey { get; set; }

    public required string LayoutKey { get; set; }

    public string? Eyebrow { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageAltText { get; set; }

    public required string BackgroundClass { get; set; }

    public required string EyebrowColorClass { get; set; }

    public required string TitleColorClass { get; set; }

    public required string DescriptionColorClass { get; set; }

    public int SortOrder { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<PageSectionButton> Buttons { get; set; } = [];

    public List<PageSectionCard> Cards { get; set; } = [];

    public List<PageSectionTestimonial> Testimonials { get; set; } = [];

    public List<PageSectionPortfolioImage> PortfolioImages { get; set; } = [];
}
