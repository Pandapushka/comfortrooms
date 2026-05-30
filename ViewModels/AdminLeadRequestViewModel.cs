namespace ComfortRooms.ViewModels;

public sealed class AdminLeadRequestViewModel
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required string Phone { get; init; }

    public string? Message { get; init; }

    public required string SourcePageSlug { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
