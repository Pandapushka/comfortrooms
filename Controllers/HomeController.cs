using System.Diagnostics;
using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ComfortRooms.Models;

namespace ComfortRooms.Controllers;

public class HomeController(IPageContentService pageContentService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var textBlocks = await pageContentService.GetTextBlocksAsync(PageSlugs.Home, cancellationToken);
        var images = await pageContentService.GetPageImagesAsync(PageSlugs.Home, cancellationToken);
        var testimonials = await pageContentService.GetHomeTestimonialsAsync(cancellationToken);
        var heroImage = images.FirstOrDefault();

        var model = new HomePageViewModel
        {
            HeroEyebrow = Text(textBlocks, "hero-eyebrow", "Премиальный свет для интерьеров"),
            HeroEyebrowColorClass = ColorClass(textBlocks, "hero-eyebrow-color", "text-accent-gold"),
            HeroTitle = Text(textBlocks, "hero-title", "Свет, который становится частью архитектуры"),
            HeroDescription = Text(textBlocks, "hero-description", "Comfort Rooms создает выразительные световые решения для частных интерьеров, дизайнерских проектов, салонов и онлайн-партнеров."),
            HeroImageUrl = heroImage?.ImageUrl ?? "/images/fallbacks/luxury-chandelier-interior.png",
            HeroImageAlt = heroImage?.AltText ?? "Люстра Comfort Rooms в интерьере",
            HeroPrimaryButtonText = Text(textBlocks, "hero-primary-button-text", "Изготовление под заказ"),
            HeroPrimaryButtonStyleClass = ButtonClass(textBlocks, "hero-primary-button-style", "button--primary"),
            HeroSecondaryButtonText = Text(textBlocks, "hero-secondary-button-text", "Связаться"),
            HeroSecondaryButtonStyleClass = ButtonClass(textBlocks, "hero-secondary-button-style", "button--secondary"),
            DirectionsEyebrow = Text(textBlocks, "directions-eyebrow", "Направления"),
            DirectionsTitle = Text(textBlocks, "directions-title", "Основные разделы сайта"),
            DirectionCards =
            [
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/na-zakaz", Title = Text(textBlocks, "direction-custom-title", "Люстры под заказ"), Description = Text(textBlocks, "direction-custom-description", "Индивидуальные светильники по чертежам, фото, эскизам и дизайнерским задачам.") },
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/dizayneram", Title = Text(textBlocks, "direction-designers-title", "Дизайнерам"), Description = Text(textBlocks, "direction-designers-description", "Партнерство для интерьерных проектов, комплектации объектов и авторских изделий.") },
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/magazinam", Title = Text(textBlocks, "direction-shops-title", "Магазинам"), Description = Text(textBlocks, "direction-shops-description", "Структура для салонов света, розничных партнеров и демонстрационных коллекций.") },
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/internet-magazinam", Title = Text(textBlocks, "direction-ecommerce-title", "Интернет-магазинам"), Description = Text(textBlocks, "direction-ecommerce-description", "Контентная и коммерческая база для будущего e-commerce сотрудничества.") },
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/opt", Title = Text(textBlocks, "direction-wholesale-title", "Оптовым партнерам"), Description = Text(textBlocks, "direction-wholesale-description", "Условия для поставок, складских остатков, регионов и регулярных закупок.") },
                new HomeDirectionCardViewModel { Url = "/sotrudnichestvo/roznitsa", Title = Text(textBlocks, "direction-retail-title", "Розничным клиентам"), Description = Text(textBlocks, "direction-retail-description", "Подбор светильников для дома, покупка online/offline, доставка и монтаж.") },
                new HomeDirectionCardViewModel { Url = "/kontakty", Title = Text(textBlocks, "direction-tenders-title", "Тендеры и аукционы"), Description = Text(textBlocks, "direction-tenders-description", "Обсуждаем проектные поставки, комплектацию объектов и документацию.") }
            ],
            ApproachEyebrow = Text(textBlocks, "approach-eyebrow", "Подход"),
            ApproachTitle = Text(textBlocks, "approach-title", "Единый стиль для всех страниц"),
            ApproachDescription = Text(textBlocks, "approach-description", "Визуальная система строится вокруг готовой страницы «Изготовление люстр под заказ»: светлый фон, золотые акценты, крупная типографика, аккуратные карточки, галереи и спокойные анимации."),
            Features =
            [
                new HomeFeatureViewModel { Title = Text(textBlocks, "feature-gallery-title", "Галереи из админки"), Description = Text(textBlocks, "feature-gallery-description", "Изображения страниц хранятся в базе и управляются отдельно по разделам.") },
                new HomeFeatureViewModel { Title = Text(textBlocks, "feature-leads-title", "Заявки сохраняются"), Description = Text(textBlocks, "feature-leads-description", "Форма на странице «под заказ» уже пишет обращения в SQLite.") },
                new HomeFeatureViewModel { Title = Text(textBlocks, "feature-ready-title", "Готово к расширению"), Description = Text(textBlocks, "feature-ready-description", "Сервисы хранения изображений сразу разделены под будущий переход на S3.") }
            ],
            CtaEyebrow = Text(textBlocks, "cta-eyebrow", "Проектирование света"),
            CtaTitle = Text(textBlocks, "cta-title", "Нужен светильник под конкретный интерьер?"),
            CtaDescription = Text(textBlocks, "cta-description", "Оставьте заявку на расчет индивидуального изделия или перейдите к странице с подробным процессом работы."),
            CtaButtonText = Text(textBlocks, "cta-button-text", "Оставить заявку"),
            CtaButtonStyleClass = ButtonClass(textBlocks, "cta-button-style", "button--primary"),
            TestimonialsEyebrow = Text(textBlocks, "testimonials-eyebrow", "Отзывы"),
            TestimonialsTitle = Text(textBlocks, "testimonials-title", "Что говорят клиенты и партнеры"),
            Testimonials = testimonials
        };

        return View(model);
    }

    [HttpGet("o-kompanii")]
    public async Task<IActionResult> About(CancellationToken cancellationToken)
    {
        var textBlocks = await pageContentService.GetTextBlocksAsync(PageSlugs.About, cancellationToken);
        var images = await pageContentService.GetPageImagesAsync(PageSlugs.About, cancellationToken);
        ViewData["TextBlocks"] = textBlocks;
        ViewData["HeroTitle"] = PageContentService.GetText(textBlocks, "hero-title", "О компании");
        ViewData["HeroDescription"] = PageContentService.GetText(textBlocks, "hero-description", "Comfort Rooms работает со светом как с архитектурным акцентом: помогает подобрать готовые решения, спроектировать индивидуальные изделия и поддержать интерьерные проекты на всех этапах.");
        ViewData["HeroImageUrl"] = images.FirstOrDefault()?.ImageUrl ?? "/images/fallbacks/luxury-chandelier-interior.png";
        ViewData["HeroImageAlt"] = images.FirstOrDefault()?.AltText ?? "Интерьерная люстра Comfort Rooms";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static string Text(IReadOnlyDictionary<string, string> blocks, string key, string fallback)
    {
        return PageContentService.GetText(blocks, key, fallback);
    }

    private static string ColorClass(IReadOnlyDictionary<string, string> blocks, string key, string fallback)
    {
        var value = Text(blocks, key, fallback);
        return value is "text-accent-gold" or "text-accent-charcoal" or "text-accent-terracotta" or "text-accent-sage"
            ? value
            : fallback;
    }

    private static string ButtonClass(IReadOnlyDictionary<string, string> blocks, string key, string fallback)
    {
        var value = Text(blocks, key, fallback);
        return value is "button--primary" or "button--secondary" or "button--gold" or "button--sage"
            ? value
            : fallback;
    }
}
