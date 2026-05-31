namespace ComfortRooms.ViewModels;

public sealed class ContactsPageViewModel
{
    public string HeroTitle { get; init; } = "Контакты";

    public string HeroDescription { get; init; } = "Обсудим индивидуальный светильник, партнерство, поставки или комплектацию проекта.";

    public LeadRequestFormViewModel LeadRequest { get; init; } = new();
}
