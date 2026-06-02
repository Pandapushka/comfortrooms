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
[Route("admin/home/testimonials")]
public sealed class HomeTestimonialsController(ComfortRoomsDbContext dbContext, IImageStorageService imageStorageService) : Controller
{
    private const string UploadFolder = "home-testimonials";

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var testimonials = await dbContext.HomeTestimonials
            .AsNoTracking()
            .OrderBy(testimonial => testimonial.SortOrder)
            .ThenBy(testimonial => testimonial.Id)
            .Select(testimonial => new AdminHomeTestimonialItemViewModel
            {
                Id = testimonial.Id,
                Title = testimonial.Title,
                Text = testimonial.Text,
                Author = testimonial.Author,
                ImageUrl = testimonial.ImageUrl,
                AltText = testimonial.AltText,
                SortOrder = testimonial.SortOrder,
                IsPublished = testimonial.IsPublished
            })
            .ToListAsync(cancellationToken);

        return View(new AdminHomeTestimonialsViewModel { Testimonials = testimonials });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string text, string? author, string? altText, IFormFile? image, CancellationToken cancellationToken)
    {
        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение для отзыва.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var imageUrl = await imageStorageService.SaveImageAsync(image, UploadFolder, cancellationToken);
            var nextSortOrder = await dbContext.HomeTestimonials.AnyAsync(cancellationToken)
                ? await dbContext.HomeTestimonials.MaxAsync(testimonial => testimonial.SortOrder, cancellationToken) + 10
                : 10;

            dbContext.HomeTestimonials.Add(new HomeTestimonial
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Новый отзыв" : title.Trim(),
                Text = string.IsNullOrWhiteSpace(text) ? "Текст отзыва" : text.Trim(),
                Author = string.IsNullOrWhiteSpace(author) ? null : author.Trim(),
                AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim(),
                ImageUrl = imageUrl,
                SortOrder = nextSortOrder,
                IsPublished = true
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            TempData["AdminMessage"] = "Отзыв добавлен.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, string title, string text, string? author, string? altText, bool isPublished, CancellationToken cancellationToken)
    {
        var testimonial = await dbContext.HomeTestimonials.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        testimonial.Title = string.IsNullOrWhiteSpace(title) ? "Отзыв" : title.Trim();
        testimonial.Text = string.IsNullOrWhiteSpace(text) ? "Текст отзыва" : text.Trim();
        testimonial.Author = string.IsNullOrWhiteSpace(author) ? null : author.Trim();
        testimonial.AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim();
        testimonial.IsPublished = isPublished;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Отзыв обновлен.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/replace")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplaceImage(int id, IFormFile? image, CancellationToken cancellationToken)
    {
        var testimonial = await dbContext.HomeTestimonials.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите новое изображение.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var previousImageUrl = testimonial.ImageUrl;
            testimonial.ImageUrl = await imageStorageService.SaveImageAsync(image, UploadFolder, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await imageStorageService.DeleteImageAsync(previousImageUrl, cancellationToken);
            TempData["AdminMessage"] = "Изображение отзыва заменено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var testimonial = await dbContext.HomeTestimonials.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        dbContext.HomeTestimonials.Remove(testimonial);
        await dbContext.SaveChangesAsync(cancellationToken);
        await imageStorageService.DeleteImageAsync(testimonial.ImageUrl, cancellationToken);
        TempData["AdminMessage"] = "Отзыв удален.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/move")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(int id, string direction, CancellationToken cancellationToken)
    {
        var testimonials = await dbContext.HomeTestimonials
            .OrderBy(testimonial => testimonial.SortOrder)
            .ThenBy(testimonial => testimonial.Id)
            .ToListAsync(cancellationToken);

        var currentIndex = testimonials.FindIndex(testimonial => testimonial.Id == id);
        if (currentIndex < 0)
        {
            return NotFound();
        }

        var targetIndex = direction == "up" ? currentIndex - 1 : currentIndex + 1;
        if (targetIndex < 0 || targetIndex >= testimonials.Count)
        {
            return RedirectToAction(nameof(Index));
        }

        (testimonials[currentIndex].SortOrder, testimonials[targetIndex].SortOrder) =
            (testimonials[targetIndex].SortOrder, testimonials[currentIndex].SortOrder);

        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
