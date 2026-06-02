namespace ComfortRooms.ViewModels;

public sealed class HomePageViewModel
{
    public string HeroEyebrow { get; init; } = "Премиальный свет для интерьеров";

    public string HeroEyebrowColorClass { get; init; } = "text-accent-gold";

    public string HeroBackgroundClass { get; init; } = "surface-cream";

    public string HeroTitle { get; init; } = "Свет, который становится частью архитектуры";

    public string HeroTitleColorClass { get; init; } = "text-accent-charcoal";

    public string HeroDescription { get; init; } = "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров.";

    public string HeroDescriptionColorClass { get; init; } = "text-accent-warm-gray";

    public string HeroImageUrl { get; init; } = "/images/fallbacks/luxury-chandelier-interior.png";

    public string HeroImageAlt { get; init; } = "Люстра Comfort Rooms в интерьере";

    public string HeroPrimaryButtonText { get; init; } = "Изготовление под заказ";

    public string HeroPrimaryButtonStyleClass { get; init; } = "button--primary";

    public string HeroSecondaryButtonText { get; init; } = "Связаться";

    public string HeroSecondaryButtonStyleClass { get; init; } = "button--secondary";

    public string DirectionsEyebrow { get; init; } = "Направления";

    public string DirectionsBackgroundClass { get; init; } = "surface-white";

    public string DirectionsEyebrowColorClass { get; init; } = "text-accent-gold";

    public string DirectionsTitle { get; init; } = "Основные разделы сайта";

    public string DirectionsTitleColorClass { get; init; } = "text-accent-charcoal";

    public IReadOnlyList<HomeDirectionCardViewModel> DirectionCards { get; init; } = [];

    public string ApproachEyebrow { get; init; } = "Подход";

    public string ApproachBackgroundClass { get; init; } = "surface-cream";

    public string ApproachEyebrowColorClass { get; init; } = "text-accent-gold";

    public string ApproachTitle { get; init; } = "Единый стиль для всех страниц";

    public string ApproachTitleColorClass { get; init; } = "text-accent-charcoal";

    public string ApproachDescription { get; init; } = "Визуальная система строится вокруг готовой страницы «Изготовление люстр под заказ»: светлый фон, золотые акценты, крупная типографика, аккуратные карточки, галереи и спокойные анимации.";

    public string ApproachDescriptionColorClass { get; init; } = "text-accent-warm-gray";

    public IReadOnlyList<HomeFeatureViewModel> Features { get; init; } = [];

    public string CtaEyebrow { get; init; } = "Проектирование света";

    public string CtaBackgroundClass { get; init; } = "surface-cream";

    public string CtaEyebrowColorClass { get; init; } = "text-accent-gold";

    public string CtaTitle { get; init; } = "Нужен светильник под конкретный интерьер?";

    public string CtaTitleColorClass { get; init; } = "text-accent-charcoal";

    public string CtaDescription { get; init; } = "Оставьте заявку на расчет индивидуального изделия или перейдите к странице с подробным процессом работы.";

    public string CtaDescriptionColorClass { get; init; } = "text-accent-warm-gray";

    public string CtaButtonText { get; init; } = "Оставить заявку";

    public string CtaButtonStyleClass { get; init; } = "button--primary";

    public string TestimonialsEyebrow { get; init; } = "Отзывы";

    public string TestimonialsBackgroundClass { get; init; } = "surface-white";

    public string TestimonialsEyebrowColorClass { get; init; } = "text-accent-gold";

    public string TestimonialsTitle { get; init; } = "Что говорят клиенты и партнеры";

    public string TestimonialsTitleColorClass { get; init; } = "text-accent-charcoal";

    public IReadOnlyList<HomeTestimonialViewModel> Testimonials { get; init; } = [];
}
