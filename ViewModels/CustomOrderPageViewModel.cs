namespace ComfortRooms.ViewModels;

public sealed class CustomOrderPageViewModel
{
    public IReadOnlyList<GalleryImageViewModel> GalleryImages { get; init; } = [];

    public LeadRequestFormViewModel LeadRequest { get; init; } = new();
}
