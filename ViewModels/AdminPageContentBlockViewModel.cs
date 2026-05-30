namespace ComfortRooms.ViewModels;

public sealed class AdminPageContentBlockViewModel
{
    public int Id { get; init; }

    public required string Key { get; init; }

    public required string Label { get; init; }

    public required string Value { get; init; }

    public int SortOrder { get; init; }
}
