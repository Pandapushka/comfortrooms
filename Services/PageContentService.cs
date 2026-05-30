using ComfortRooms.Data;
using ComfortRooms.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Services;

public sealed class PageContentService(ComfortRoomsDbContext dbContext) : IPageContentService
{
    public async Task<CustomOrderPageViewModel> GetCustomOrderPageAsync(CancellationToken cancellationToken)
    {
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
            GalleryImages = images
        };
    }
}
