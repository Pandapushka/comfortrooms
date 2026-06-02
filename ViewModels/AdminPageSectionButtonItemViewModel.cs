namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionButtonItemViewModel
{
    public int Id { get; init; }

    public required string Text { get; init; }

    public required string Url { get; init; }

    public required string StyleClass { get; init; }

    public int SortOrder { get; init; }
}
