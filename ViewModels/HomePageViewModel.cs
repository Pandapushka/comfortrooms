namespace ComfortRooms.ViewModels;

public sealed class HomePageViewModel
{
    public string HeroEyebrow { get; init; } = "Премиальный свет для интерьеров";

    public string HeroEyebrowColorClass { get; init; } = "text-accent-gold";

    public string HeroTitle { get; init; } = "Свет, который становится частью архитектуры";

    public string HeroDescription { get; init; } = "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров.";

    public string HeroImageUrl { get; init; } = "/images/fallbacks/luxury-chandelier-interior.png";

    public string HeroImageAlt { get; init; } = "Люстра Comfort Rooms в интерьере";

    public string HeroPrimaryButtonText { get; init; } = "Изготовление под заказ";

    public string HeroPrimaryButtonStyleClass { get; init; } = "button--primary";

    public string HeroSecondaryButtonText { get; init; } = "Связаться";

    public string HeroSecondaryButtonStyleClass { get; init; } = "button--secondary";

    public string DirectionsEyebrow { get; init; } = "Направления";

    public string DirectionsTitle { get; init; } = "Основные разделы сайта";

    public IReadOnlyList<HomeDirectionCardViewModel> DirectionCards { get; init; } = [];

    public string ApproachEyebrow { get; init; } = "Подход";

    public string ApproachTitle { get; init; } = "Единый стиль для всех страниц";

    public string ApproachDescription { get; init; } = "Визуальная система строится вокруг готовой страницы «Изготовление люстр под заказ»: светлый фон, золотые акценты, крупная типографика, аккуратные карточки, галереи и спокойные анимации.";

    public IReadOnlyList<HomeFeatureViewModel> Features { get; init; } = [];

    public string CtaEyebrow { get; init; } = "Проектирование света";

    public string CtaTitle { get; init; } = "Нужен светильник под конкретный интерьер?";

    public string CtaDescription { get; init; } = "Оставьте заявку на расчет индивидуального изделия или перейдите к странице с подробным процессом работы.";

    public string CtaButtonText { get; init; } = "Оставить заявку";

    public string CtaButtonStyleClass { get; init; } = "button--primary";

    public string TestimonialsEyebrow { get; init; } = "Отзывы";

    public string TestimonialsTitle { get; init; } = "Что говорят клиенты и партнеры";

    public IReadOnlyList<HomeTestimonialViewModel> Testimonials { get; init; } = [];
}
