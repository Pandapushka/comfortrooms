using ComfortRooms.Data;
using ComfortRooms.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Services;

public sealed class PageContentService(ComfortRoomsDbContext dbContext) : IPageContentService
{
    public async Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken)
    {
        var textBlocks = await GetTextBlocksAsync(PageSlugs.CustomOrder, cancellationToken);
        var images = await GetPageImagesAsync(PageSlugs.CustomOrder, cancellationToken);

        return new CustomOrderPageViewModel
        {
            HeroTitle = GetText(textBlocks, "hero-title", "Изготовление люстр под заказ"),
            HeroDescription = GetText(textBlocks, "hero-description", "От идеи до воплощения: эксклюзивные светильники для дизайнеров интерьеров, архитекторов и частных заказчиков."),
            TextBlocks = textBlocks,
            GalleryImages = images.Count > 0 ? images : DefaultCustomOrderGallery()
        };
    }

    public async Task<IReadOnlyList<GalleryImageViewModel>> GetPageImagesAsync(string pageSlug, CancellationToken cancellationToken)
    {
        var images = await dbContext.PageImages
            .AsNoTracking()
            .Where(image => image.SitePage != null && image.SitePage.Slug == pageSlug)
            .OrderBy(image => image.SortOrder)
            .ThenBy(image => image.Id)
            .Select(image => new GalleryImageViewModel
            {
                Title = image.Title,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText
            })
            .ToListAsync(cancellationToken);

        return images
            .Select(image => new GalleryImageViewModel
            {
                Title = image.Title,
                ImageUrl = NormalizeImageUrl(image.ImageUrl),
                AltText = image.AltText
            })
            .ToList();
    }

    public async Task<IReadOnlyDictionary<string, string>> GetTextBlocksAsync(string pageSlug, CancellationToken cancellationToken)
    {
        return await dbContext.PageContentBlocks
            .AsNoTracking()
            .Where(block => block.SitePage != null && block.SitePage.Slug == pageSlug)
            .OrderBy(block => block.SortOrder)
            .ToDictionaryAsync(block => block.Key, block => block.Value, StringComparer.OrdinalIgnoreCase, cancellationToken);
    }

    public async Task<IReadOnlyList<HomeTestimonialViewModel>> GetHomeTestimonialsAsync(CancellationToken cancellationToken)
    {
        var testimonials = await dbContext.HomeTestimonials
            .AsNoTracking()
            .Where(testimonial => testimonial.IsPublished)
            .OrderBy(testimonial => testimonial.SortOrder)
            .ThenBy(testimonial => testimonial.Id)
            .Select(testimonial => new HomeTestimonialViewModel
            {
                Title = testimonial.Title,
                Text = testimonial.Text,
                Author = testimonial.Author,
                ImageUrl = testimonial.ImageUrl,
                AltText = testimonial.AltText
            })
            .ToListAsync(cancellationToken);

        return testimonials.Count > 0 ? testimonials : DefaultHomeTestimonials();
    }

    public async Task<IReadOnlyList<PageSectionViewModel>> GetPageSectionsAsync(string pageSlug, CancellationToken cancellationToken)
    {
        return await dbContext.PageSections
            .AsNoTracking()
            .AsSplitQuery()
            .Where(section => section.SitePage != null && section.SitePage.Slug == pageSlug && section.IsPublished)
            .OrderBy(section => section.SortOrder)
            .ThenBy(section => section.Id)
            .Select(section => new PageSectionViewModel
            {
                TemplateKey = section.TemplateKey,
                LayoutKey = section.LayoutKey,
                Eyebrow = section.Eyebrow,
                Title = section.Title,
                Description = section.Description,
                ImageUrl = section.ImageUrl,
                ImageAltText = section.ImageAltText,
                BackgroundClass = section.BackgroundClass,
                EyebrowColorClass = section.EyebrowColorClass,
                TitleColorClass = section.TitleColorClass,
                DescriptionColorClass = section.DescriptionColorClass,
                Buttons = section.Buttons
                    .OrderBy(button => button.SortOrder)
                    .ThenBy(button => button.Id)
                    .Select(button => new PageSectionButtonViewModel
                    {
                        Text = button.Text,
                        Url = button.Url,
                        StyleClass = button.StyleClass
                    })
                    .ToList(),
                Cards = section.Cards
                    .OrderBy(card => card.SortOrder)
                    .ThenBy(card => card.Id)
                    .Select(card => new PageSectionCardViewModel
                    {
                        Title = card.Title,
                        Description = card.Description,
                        IsLink = card.IsLink,
                        Url = card.Url
                    })
                    .ToList(),
                Testimonials = section.Testimonials
                    .Where(testimonial => testimonial.IsPublished)
                    .OrderBy(testimonial => testimonial.SortOrder)
                    .ThenBy(testimonial => testimonial.Id)
                    .Select(testimonial => new PageSectionTestimonialViewModel
                    {
                        Title = testimonial.Title,
                        Text = testimonial.Text,
                        Author = testimonial.Author,
                        ImageUrl = testimonial.ImageUrl,
                        AltText = testimonial.AltText
                    })
                    .ToList(),
                PortfolioImages = section.PortfolioImages
                    .OrderBy(image => image.SortOrder)
                    .ThenBy(image => image.Id)
                    .Select(image => new PageSectionPortfolioImageViewModel
                    {
                        Title = image.Title,
                        ImageUrl = image.ImageUrl,
                        AltText = image.AltText
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public static string GetText(IReadOnlyDictionary<string, string> blocks, string key, string fallback)
    {
        return blocks.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }

    public async Task<IReadOnlyList<GalleryImageViewModel>> GetGalleryImagesAsync(string pageSlug, CancellationToken cancellationToken)
    {
        return await GetPageImagesAsync(pageSlug, cancellationToken);
    }

    public static IReadOnlyList<GalleryImageViewModel> DefaultCustomOrderGallery()
    {
        const string fallbackImage = "/images/fallbacks/luxury-chandelier-interior.png";

        return
        [
            new GalleryImageViewModel { Title = "Индивидуальная люстра", ImageUrl = fallbackImage, AltText = "Индивидуальная люстра в интерьере" },
            new GalleryImageViewModel { Title = "Проектный свет", ImageUrl = fallbackImage, AltText = "Проектный светильник Comfort Rooms" },
            new GalleryImageViewModel { Title = "Латунь и стекло", ImageUrl = fallbackImage, AltText = "Люстра с латунными и стеклянными деталями" }
        ];
    }

    private static string NormalizeImageUrl(string imageUrl)
    {
        return imageUrl.Contains("image.qwenlm.ai", StringComparison.OrdinalIgnoreCase)
            ? "/images/fallbacks/luxury-chandelier-interior.png"
            : imageUrl;
    }

    private static IReadOnlyList<HomeTestimonialViewModel> DefaultHomeTestimonials()
    {
        const string fallbackImage = "/images/fallbacks/luxury-chandelier-interior.png";

        return
        [
            new HomeTestimonialViewModel
            {
                Title = "Частный интерьер",
                Text = "Comfort Rooms помогли подобрать масштабный светильник под готовый интерьер и аккуратно довели идею до результата.",
                Author = "Клиент Comfort Rooms",
                ImageUrl = fallbackImage,
                AltText = "Отзыв клиента Comfort Rooms"
            },
            new HomeTestimonialViewModel
            {
                Title = "Проект дизайнера",
                Text = "Команда быстро включилась в задачу, уточнила материалы, размеры и подготовила понятный путь изготовления.",
                Author = "Дизайнер интерьера",
                ImageUrl = fallbackImage,
                AltText = "Отзыв дизайнера Comfort Rooms"
            },
            new HomeTestimonialViewModel
            {
                Title = "Партнерская поставка",
                Text = "Для партнерских заказов важны сроки, коммуникация и документы. Здесь все этапы были прозрачными.",
                Author = "Партнер Comfort Rooms",
                ImageUrl = fallbackImage,
                AltText = "Отзыв партнера Comfort Rooms"
            }
        ];
    }
}
