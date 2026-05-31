using ComfortRooms.Data;
using ComfortRooms.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Services;

public sealed class PageContentService(ComfortRoomsDbContext dbContext) : IPageContentService
{
    public async Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken)
    {
        var textBlocks = await GetTextBlocksAsync(PageSlugs.CustomOrder, cancellationToken);
        var images = await dbContext.PageImages
            .AsNoTracking()
            .Where(image => image.SitePage != null && image.SitePage.Slug == PageSlugs.CustomOrder)
            .OrderBy(image => image.SortOrder)
            .ThenBy(image => image.Id)
            .Select(image => new GalleryImageViewModel
            {
                Title = image.Title,
                ImageUrl = image.ImageUrl,
                AltText = image.AltText
            })
            .ToListAsync(cancellationToken);

        return new CustomOrderPageViewModel
        {
            HeroTitle = GetText(textBlocks, "hero-title", "Изготовление люстр под заказ"),
            HeroDescription = GetText(textBlocks, "hero-description", "От идеи до воплощения: эксклюзивные светильники для дизайнеров интерьеров, архитекторов и частных заказчиков."),
            TextBlocks = textBlocks,
            GalleryImages = images
        };
    }

    public async Task<IReadOnlyDictionary<string, string>> GetTextBlocksAsync(string pageSlug, CancellationToken cancellationToken)
    {
        return await dbContext.PageContentBlocks
            .AsNoTracking()
            .Where(block => block.SitePage != null && block.SitePage.Slug == pageSlug)
            .OrderBy(block => block.SortOrder)
            .ToDictionaryAsync(block => block.Key, block => block.Value, StringComparer.OrdinalIgnoreCase, cancellationToken);
    }

    public static string GetText(IReadOnlyDictionary<string, string> blocks, string key, string fallback)
    {
        return blocks.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }
}
