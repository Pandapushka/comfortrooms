namespace ComfortRooms.ViewModels;

public sealed class CooperationAudiencePageViewModel
{
    public required string Title { get; init; }

    public required string Eyebrow { get; init; }

    public required string Description { get; init; }

    public required string Accent { get; init; }

    public required string ImageUrl { get; init; }

    public required string ImageAlt { get; init; }

    public IReadOnlyList<CooperationAudienceCardViewModel> Cards { get; init; } = [];

    public IReadOnlyList<string> Steps { get; init; } = [];
}
