namespace ComfortRooms.ViewModels;

public sealed class CooperationAudiencePageViewModel
{
    public required string PageSlug { get; init; }

    public required string Title { get; set; }

    public required string Eyebrow { get; init; }

    public required string Description { get; set; }

    public required string Accent { get; init; }

    public required string ImageUrl { get; init; }

    public required string ImageAlt { get; init; }

    public IReadOnlyDictionary<string, string> TextBlocks { get; set; } = new Dictionary<string, string>();

    public IReadOnlyList<CooperationAudienceCardViewModel> Cards { get; init; } = [];

    public IReadOnlyList<string> Steps { get; init; } = [];
}
