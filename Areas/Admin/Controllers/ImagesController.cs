using ComfortRooms.Data;
using ComfortRooms.Models;
using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/pages")]
public sealed class ImagesController(ComfortRoomsDbContext dbContext, IImageStorageService imageStorageService) : Controller
{
    [HttpGet("{slug}/images")]
    public async Task<IActionResult> Index(string slug)
    {
        var page = await dbContext.SitePages
            .AsNoTracking()
            .Include(sitePage => sitePage.Images.OrderBy(image => image.SortOrder))
            .SingleOrDefaultAsync(sitePage => sitePage.Slug == slug);

        if (page is null)
        {
            return NotFound();
        }

        var model = new AdminPageImagesViewModel
        {
            PageSlug = page.Slug,
            PageTitle = page.Title,
            Images = page.Images
                .OrderBy(image => image.SortOrder)
                .ThenBy(image => image.Id)
                .Select(image => new AdminPageImageItemViewModel
                {
                    Id = image.Id,
                    Title = image.Title,
                    ImageUrl = image.ImageUrl,
                    AltText = image.AltText,
                    SortOrder = image.SortOrder
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost("{slug}/images")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(string slug, string title, string? altText, IFormFile image, CancellationToken cancellationToken)
    {
        var page = await dbContext.SitePages
            .Include(sitePage => sitePage.Images)
            .SingleOrDefaultAsync(sitePage => sitePage.Slug == slug, cancellationToken);

        if (page is null)
        {
            return NotFound();
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var imageUrl = await imageStorageService.SaveImageAsync(image, slug, cancellationToken);
            var nextSortOrder = page.Images.Count == 0 ? 10 : page.Images.Max(item => item.SortOrder) + 10;

            page.Images.Add(new PageImage
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Новое изображение" : title.Trim(),
                AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim(),
                ImageUrl = imageUrl,
                SortOrder = nextSortOrder
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            TempData["AdminMessage"] = "Изображение добавлено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{slug}/images/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, int id)
    {
        var image = await dbContext.PageImages
            .Include(item => item.SitePage)
            .SingleOrDefaultAsync(item => item.Id == id && item.SitePage != null && item.SitePage.Slug == slug);

        if (image is null)
        {
            return NotFound();
        }

        dbContext.PageImages.Remove(image);
        await dbContext.SaveChangesAsync();
        await imageStorageService.DeleteImageAsync(image.ImageUrl, HttpContext.RequestAborted);
        TempData["AdminMessage"] = "Изображение удалено.";

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{slug}/images/{id:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string slug, int id, string title, string? altText)
    {
        var image = await dbContext.PageImages
            .Include(item => item.SitePage)
            .SingleOrDefaultAsync(item => item.Id == id && item.SitePage != null && item.SitePage.Slug == slug);

        if (image is null)
        {
            return NotFound();
        }

        image.Title = string.IsNullOrWhiteSpace(title) ? "Изображение" : title.Trim();
        image.AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim();

        await dbContext.SaveChangesAsync();
        TempData["AdminMessage"] = "Описание изображения обновлено.";

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{slug}/images/{id:int}/replace")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Replace(string slug, int id, IFormFile image, CancellationToken cancellationToken)
    {
        var pageImage = await dbContext.PageImages
            .Include(item => item.SitePage)
            .SingleOrDefaultAsync(item => item.Id == id && item.SitePage != null && item.SitePage.Slug == slug, cancellationToken);

        if (pageImage is null)
        {
            return NotFound();
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите новое изображение.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var previousImageUrl = pageImage.ImageUrl;
            var newImageUrl = await imageStorageService.SaveImageAsync(image, slug, cancellationToken);
            pageImage.ImageUrl = newImageUrl;

            await dbContext.SaveChangesAsync(cancellationToken);
            await imageStorageService.DeleteImageAsync(previousImageUrl, cancellationToken);
            TempData["AdminMessage"] = "Изображение заменено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{slug}/images/{id:int}/move")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(string slug, int id, string direction)
    {
        var page = await dbContext.SitePages
            .Include(sitePage => sitePage.Images)
            .SingleOrDefaultAsync(sitePage => sitePage.Slug == slug);

        if (page is null)
        {
            return NotFound();
        }

        var images = page.Images.OrderBy(image => image.SortOrder).ThenBy(image => image.Id).ToList();
        var currentIndex = images.FindIndex(image => image.Id == id);
        if (currentIndex < 0)
        {
            return NotFound();
        }

        var targetIndex = direction == "up" ? currentIndex - 1 : currentIndex + 1;
        if (targetIndex < 0 || targetIndex >= images.Count)
        {
            return RedirectToAction(nameof(Index), new { slug });
        }

        (images[currentIndex].SortOrder, images[targetIndex].SortOrder) = (images[targetIndex].SortOrder, images[currentIndex].SortOrder);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { slug });
    }
}
