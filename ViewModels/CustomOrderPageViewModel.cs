namespace ComfortRooms.ViewModels;

public sealed class CustomOrderPageViewModel
{
    public string HeroTitle { get; init; } = "Изготовление люстр под заказ";

    public string HeroDescription { get; init; } = "От идеи до воплощения: эксклюзивные светильники для дизайнеров интерьеров, архитекторов и частных заказчиков.";

    public IReadOnlyDictionary<string, string> TextBlocks { get; init; } = new Dictionary<string, string>();

    public IReadOnlyList<GalleryImageViewModel> GalleryImages { get; init; } = [];

    public LeadRequestFormViewModel LeadRequest { get; init; } = new();
}
