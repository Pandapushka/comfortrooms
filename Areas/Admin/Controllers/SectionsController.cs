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
[Route("admin/pages/{slug}/sections")]
public sealed class SectionsController(ComfortRoomsDbContext dbContext, IImageStorageService imageStorageService) : Controller
{
    private const string TextImageTemplate = "text-image";
    private const string CardsGridTemplate = "cards-grid";
    private const string TestimonialsTemplate = "testimonials";
    private const string MapTemplate = "map";
    private const string PortfolioTemplate = "portfolio";

    [HttpGet("")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        var page = await dbContext.SitePages
            .AsNoTracking()
            .Include(sitePage => sitePage.Sections.OrderBy(section => section.SortOrder).ThenBy(section => section.Id))
                .ThenInclude(section => section.Buttons.OrderBy(button => button.SortOrder).ThenBy(button => button.Id))
            .Include(sitePage => sitePage.Sections.OrderBy(section => section.SortOrder).ThenBy(section => section.Id))
                .ThenInclude(section => section.Cards.OrderBy(card => card.SortOrder).ThenBy(card => card.Id))
            .Include(sitePage => sitePage.Sections.OrderBy(section => section.SortOrder).ThenBy(section => section.Id))
                .ThenInclude(section => section.Testimonials.OrderBy(testimonial => testimonial.SortOrder).ThenBy(testimonial => testimonial.Id))
            .Include(sitePage => sitePage.Sections.OrderBy(section => section.SortOrder).ThenBy(section => section.Id))
                .ThenInclude(section => section.PortfolioImages.OrderBy(image => image.SortOrder).ThenBy(image => image.Id))
            .SingleOrDefaultAsync(sitePage => sitePage.Slug == slug, cancellationToken);

        if (page is null)
        {
            return NotFound();
        }

        return View(new AdminPageSectionsViewModel
        {
            PageSlug = page.Slug,
            PageTitle = page.Title,
            TemplateOptions = TemplateOptions,
            LayoutOptions = LayoutOptions,
            BackgroundOptions = BackgroundOptions,
            TextColorOptions = TextColorOptions,
            ButtonStyleOptions = ButtonStyleOptions,
            Sections = page.Sections
                .OrderBy(section => section.SortOrder)
                .ThenBy(section => section.Id)
                .Select(section => new AdminPageSectionItemViewModel
                {
                    Id = section.Id,
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
                    SortOrder = section.SortOrder,
                    IsPublished = section.IsPublished,
                    Buttons = section.Buttons
                        .OrderBy(button => button.SortOrder)
                        .ThenBy(button => button.Id)
                        .Select(button => new AdminPageSectionButtonItemViewModel
                        {
                            Id = button.Id,
                            Text = button.Text,
                            Url = button.Url,
                            StyleClass = button.StyleClass,
                            SortOrder = button.SortOrder
                        })
                        .ToList(),
                    Cards = section.Cards
                        .OrderBy(card => card.SortOrder)
                        .ThenBy(card => card.Id)
                        .Select(card => new AdminPageSectionCardItemViewModel
                        {
                            Id = card.Id,
                            Title = card.Title,
                            Description = card.Description,
                            IsLink = card.IsLink,
                            Url = card.Url,
                            SortOrder = card.SortOrder
                        })
                        .ToList(),
                    Testimonials = section.Testimonials
                        .OrderBy(testimonial => testimonial.SortOrder)
                        .ThenBy(testimonial => testimonial.Id)
                        .Select(testimonial => new AdminPageSectionTestimonialItemViewModel
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
                        .ToList(),
                    PortfolioImages = section.PortfolioImages
                        .OrderBy(image => image.SortOrder)
                        .ThenBy(image => image.Id)
                        .Select(image => new AdminPageSectionPortfolioImageItemViewModel
                        {
                            Id = image.Id,
                            Title = image.Title,
                            ImageUrl = image.ImageUrl,
                            AltText = image.AltText,
                            SortOrder = image.SortOrder
                        })
                        .ToList()
                })
                .ToList()
        });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string slug,
        string templateKey,
        string? layoutKey,
        string title,
        string? eyebrow,
        string? description,
        string? mapEmbedHtml,
        string? imageAltText,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        var page = await dbContext.SitePages.SingleOrDefaultAsync(sitePage => sitePage.Slug == slug, cancellationToken);
        if (page is null)
        {
            return NotFound();
        }

        var normalizedTemplateKey = NormalizeTemplate(templateKey);
        string? imageUrl = null;
        if (image is not null && normalizedTemplateKey == TextImageTemplate)
        {
            try
            {
                imageUrl = await imageStorageService.SaveImageAsync(image, $"sections-{slug}", cancellationToken);
            }
            catch (InvalidOperationException exception)
            {
                TempData["AdminError"] = exception.Message;
                return RedirectToAction(nameof(Index), new { slug });
            }
        }

        var nextSortOrder = await dbContext.PageSections.AnyAsync(section => section.SitePageId == page.Id, cancellationToken)
            ? await dbContext.PageSections.Where(section => section.SitePageId == page.Id).MaxAsync(section => section.SortOrder, cancellationToken) + 10
            : 10;

        dbContext.PageSections.Add(new PageSection
        {
            SitePageId = page.Id,
            TemplateKey = normalizedTemplateKey,
            LayoutKey = normalizedTemplateKey == TextImageTemplate ? NormalizeLayout(layoutKey) : normalizedTemplateKey,
            Eyebrow = TrimOrNull(eyebrow),
            Title = normalizedTemplateKey == MapTemplate
                ? (string.IsNullOrWhiteSpace(title) ? "Карта" : title.Trim())
                : (string.IsNullOrWhiteSpace(title) ? "Новый блок" : title.Trim()),
            Description = normalizedTemplateKey == MapTemplate ? TrimOrNull(mapEmbedHtml) : TrimOrNull(description),
            ImageUrl = normalizedTemplateKey == TextImageTemplate ? imageUrl : null,
            ImageAltText = normalizedTemplateKey == TextImageTemplate ? TrimOrNull(imageAltText) : null,
            BackgroundClass = "surface-cream",
            EyebrowColorClass = "text-accent-gold",
            TitleColorClass = "text-accent-charcoal",
            DescriptionColorClass = "text-accent-warm-gray",
            SortOrder = nextSortOrder,
            IsPublished = true
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Блок добавлен.";

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        string slug,
        int id,
        string? layoutKey,
        string title,
        string? eyebrow,
        string? description,
        string? imageAltText,
        string backgroundClass,
        string eyebrowColorClass,
        string titleColorClass,
        string descriptionColorClass,
        bool isPublished,
        CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        section.LayoutKey = string.Equals(section.TemplateKey, TextImageTemplate, StringComparison.OrdinalIgnoreCase)
            ? NormalizeLayout(layoutKey)
            : section.TemplateKey;
        section.Eyebrow = TrimOrNull(eyebrow);
        section.Title = string.IsNullOrWhiteSpace(title) ? "Блок" : title.Trim();
        section.Description = TrimOrNull(description);
        section.ImageAltText = TrimOrNull(imageAltText);
        section.BackgroundClass = NormalizeBackground(backgroundClass);
        section.EyebrowColorClass = NormalizeTextColor(eyebrowColorClass);
        section.TitleColorClass = NormalizeTextColor(titleColorClass);
        section.DescriptionColorClass = NormalizeTextColor(descriptionColorClass);
        section.IsPublished = isPublished;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Блок обновлен.";

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/cards")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCard(string slug, int id, string title, string? description, bool isLink, string? url, CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        if (!string.Equals(section.TemplateKey, CardsGridTemplate, StringComparison.OrdinalIgnoreCase))
        {
            TempData["AdminError"] = "Карточки можно добавлять только в блок второго шаблона.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        var nextSortOrder = section.Cards.Count > 0 ? section.Cards.Max(card => card.SortOrder) + 10 : 10;
        section.Cards.Add(new PageSectionCard
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Новая карточка" : title.Trim(),
            Description = TrimOrNull(description),
            IsLink = isLink,
            Url = isLink ? NormalizeOptionalUrl(url) : null,
            SortOrder = nextSortOrder
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Карточка добавлена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/cards/{cardId:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCard(string slug, int sectionId, int cardId, string title, string? description, bool isLink, string? url, CancellationToken cancellationToken)
    {
        var card = await dbContext.PageSectionCards
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == cardId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);

        if (card is null)
        {
            return NotFound();
        }

        card.Title = string.IsNullOrWhiteSpace(title) ? "Карточка" : title.Trim();
        card.Description = TrimOrNull(description);
        card.IsLink = isLink;
        card.Url = isLink ? NormalizeOptionalUrl(url) : null;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Карточка обновлена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/cards/{cardId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCard(string slug, int sectionId, int cardId, CancellationToken cancellationToken)
    {
        var card = await dbContext.PageSectionCards
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == cardId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);

        if (card is null)
        {
            return NotFound();
        }

        dbContext.PageSectionCards.Remove(card);
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Карточка удалена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/testimonials")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTestimonial(
        string slug,
        int id,
        string title,
        string text,
        string? author,
        string? altText,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        if (!string.Equals(section.TemplateKey, TestimonialsTemplate, StringComparison.OrdinalIgnoreCase))
        {
            TempData["AdminError"] = "Отзывы можно добавлять только в блок третьего шаблона.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение для отзыва.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var nextSortOrder = section.Testimonials.Count > 0 ? section.Testimonials.Max(testimonial => testimonial.SortOrder) + 10 : 10;
            section.Testimonials.Add(new PageSectionTestimonial
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Отзыв" : title.Trim(),
                Text = string.IsNullOrWhiteSpace(text) ? "Текст отзыва" : text.Trim(),
                Author = TrimOrNull(author),
                ImageUrl = await imageStorageService.SaveImageAsync(image, $"section-testimonials-{slug}", cancellationToken),
                AltText = TrimOrNull(altText),
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

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/testimonials/{testimonialId:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTestimonial(
        string slug,
        int sectionId,
        int testimonialId,
        string title,
        string text,
        string? author,
        string? altText,
        bool isPublished,
        CancellationToken cancellationToken)
    {
        var testimonial = await FindTestimonialAsync(slug, sectionId, testimonialId, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        testimonial.Title = string.IsNullOrWhiteSpace(title) ? "Отзыв" : title.Trim();
        testimonial.Text = string.IsNullOrWhiteSpace(text) ? "Текст отзыва" : text.Trim();
        testimonial.Author = TrimOrNull(author);
        testimonial.AltText = TrimOrNull(altText);
        testimonial.IsPublished = isPublished;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Отзыв обновлен.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/testimonials/{testimonialId:int}/replace-image")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplaceTestimonialImage(string slug, int sectionId, int testimonialId, IFormFile? image, CancellationToken cancellationToken)
    {
        var testimonial = await FindTestimonialAsync(slug, sectionId, testimonialId, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение для отзыва.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var previousImageUrl = testimonial.ImageUrl;
            testimonial.ImageUrl = await imageStorageService.SaveImageAsync(image, $"section-testimonials-{slug}", cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await imageStorageService.DeleteImageAsync(previousImageUrl, cancellationToken);
            TempData["AdminMessage"] = "Изображение отзыва заменено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/testimonials/{testimonialId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTestimonial(string slug, int sectionId, int testimonialId, CancellationToken cancellationToken)
    {
        var testimonial = await FindTestimonialAsync(slug, sectionId, testimonialId, cancellationToken);
        if (testimonial is null)
        {
            return NotFound();
        }

        var imageUrl = testimonial.ImageUrl;
        dbContext.PageSectionTestimonials.Remove(testimonial);
        await dbContext.SaveChangesAsync(cancellationToken);
        await imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
        TempData["AdminMessage"] = "Отзыв удален.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/portfolio-images")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPortfolioImage(
        string slug,
        int id,
        string title,
        string? altText,
        IFormFile? image,
        CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        if (!string.Equals(section.TemplateKey, PortfolioTemplate, StringComparison.OrdinalIgnoreCase))
        {
            TempData["AdminError"] = "Изображения портфолио можно добавлять только в блок портфолио.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение для портфолио.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var nextSortOrder = section.PortfolioImages.Count > 0 ? section.PortfolioImages.Max(item => item.SortOrder) + 10 : 10;
            section.PortfolioImages.Add(new PageSectionPortfolioImage
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Работа Comfort Rooms" : title.Trim(),
                AltText = TrimOrNull(altText),
                ImageUrl = await imageStorageService.SaveImageAsync(image, $"section-portfolio-{slug}", cancellationToken),
                SortOrder = nextSortOrder
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            TempData["AdminMessage"] = "Изображение портфолио добавлено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/portfolio-images/{imageId:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePortfolioImage(string slug, int sectionId, int imageId, string title, string? altText, CancellationToken cancellationToken)
    {
        var image = await FindPortfolioImageAsync(slug, sectionId, imageId, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        image.Title = string.IsNullOrWhiteSpace(title) ? "Работа Comfort Rooms" : title.Trim();
        image.AltText = TrimOrNull(altText);

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Описание изображения портфолио обновлено.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/portfolio-images/{imageId:int}/replace")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplacePortfolioImage(string slug, int sectionId, int imageId, IFormFile? image, CancellationToken cancellationToken)
    {
        var portfolioImage = await FindPortfolioImageAsync(slug, sectionId, imageId, cancellationToken);
        if (portfolioImage is null)
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
            var previousImageUrl = portfolioImage.ImageUrl;
            portfolioImage.ImageUrl = await imageStorageService.SaveImageAsync(image, $"section-portfolio-{slug}", cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await imageStorageService.DeleteImageAsync(previousImageUrl, cancellationToken);
            TempData["AdminMessage"] = "Изображение портфолио заменено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/portfolio-images/{imageId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePortfolioImage(string slug, int sectionId, int imageId, CancellationToken cancellationToken)
    {
        var image = await FindPortfolioImageAsync(slug, sectionId, imageId, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        var imageUrl = image.ImageUrl;
        dbContext.PageSectionPortfolioImages.Remove(image);
        await dbContext.SaveChangesAsync(cancellationToken);
        await imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
        TempData["AdminMessage"] = "Изображение портфолио удалено.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/portfolio-images/{imageId:int}/move")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MovePortfolioImage(string slug, int sectionId, int imageId, string direction, CancellationToken cancellationToken)
    {
        var images = await dbContext.PageSectionPortfolioImages
            .Where(image => image.PageSectionId == sectionId && image.PageSection != null && image.PageSection.SitePage != null && image.PageSection.SitePage.Slug == slug)
            .OrderBy(image => image.SortOrder)
            .ThenBy(image => image.Id)
            .ToListAsync(cancellationToken);

        var currentIndex = images.FindIndex(image => image.Id == imageId);
        if (currentIndex < 0)
        {
            return NotFound();
        }

        var targetIndex = direction == "up" ? currentIndex - 1 : currentIndex + 1;
        if (targetIndex < 0 || targetIndex >= images.Count)
        {
            return RedirectToAction(nameof(Index), new { slug });
        }

        (images[currentIndex].SortOrder, images[targetIndex].SortOrder) =
            (images[targetIndex].SortOrder, images[currentIndex].SortOrder);

        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/testimonials/{testimonialId:int}/move")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoveTestimonial(string slug, int sectionId, int testimonialId, string direction, CancellationToken cancellationToken)
    {
        var testimonials = await dbContext.PageSectionTestimonials
            .Where(testimonial => testimonial.PageSectionId == sectionId && testimonial.PageSection != null && testimonial.PageSection.SitePage != null && testimonial.PageSection.SitePage.Slug == slug)
            .OrderBy(testimonial => testimonial.SortOrder)
            .ThenBy(testimonial => testimonial.Id)
            .ToListAsync(cancellationToken);

        var currentIndex = testimonials.FindIndex(testimonial => testimonial.Id == testimonialId);
        if (currentIndex < 0)
        {
            return NotFound();
        }

        var targetIndex = direction == "up" ? currentIndex - 1 : currentIndex + 1;
        if (targetIndex < 0 || targetIndex >= testimonials.Count)
        {
            return RedirectToAction(nameof(Index), new { slug });
        }

        (testimonials[currentIndex].SortOrder, testimonials[targetIndex].SortOrder) =
            (testimonials[targetIndex].SortOrder, testimonials[currentIndex].SortOrder);

        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/replace-image")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplaceImage(string slug, int id, IFormFile? image, CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        if (!string.Equals(section.TemplateKey, TextImageTemplate, StringComparison.OrdinalIgnoreCase))
        {
            TempData["AdminError"] = "Изображение используется только в первом шаблоне блока.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        if (image is null)
        {
            TempData["AdminError"] = "Выберите изображение для блока.";
            return RedirectToAction(nameof(Index), new { slug });
        }

        try
        {
            var previousImageUrl = section.ImageUrl;
            section.ImageUrl = await imageStorageService.SaveImageAsync(image, $"sections-{slug}", cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(previousImageUrl))
            {
                await imageStorageService.DeleteImageAsync(previousImageUrl, cancellationToken);
            }
            TempData["AdminMessage"] = "Изображение блока заменено.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string slug, int id, CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        var imageUrl = section.ImageUrl;
        var testimonialImageUrls = section.Testimonials
            .Select(testimonial => testimonial.ImageUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();
        var portfolioImageUrls = section.PortfolioImages
            .Select(image => image.ImageUrl)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .ToList();
        dbContext.PageSections.Remove(section);
        await dbContext.SaveChangesAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            await imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
        }
        foreach (var testimonialImageUrl in testimonialImageUrls)
        {
            await imageStorageService.DeleteImageAsync(testimonialImageUrl, cancellationToken);
        }
        foreach (var portfolioImageUrl in portfolioImageUrls)
        {
            await imageStorageService.DeleteImageAsync(portfolioImageUrl, cancellationToken);
        }

        TempData["AdminMessage"] = "Блок удален.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/move")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(string slug, int id, string direction, CancellationToken cancellationToken)
    {
        var sections = await dbContext.PageSections
            .Where(section => section.SitePage != null && section.SitePage.Slug == slug)
            .OrderBy(section => section.SortOrder)
            .ThenBy(section => section.Id)
            .ToListAsync(cancellationToken);

        var currentIndex = sections.FindIndex(section => section.Id == id);
        if (currentIndex < 0)
        {
            return NotFound();
        }

        var targetIndex = direction == "up" ? currentIndex - 1 : currentIndex + 1;
        if (targetIndex < 0 || targetIndex >= sections.Count)
        {
            return RedirectToAction(nameof(Index), new { slug });
        }

        (sections[currentIndex].SortOrder, sections[targetIndex].SortOrder) =
            (sections[targetIndex].SortOrder, sections[currentIndex].SortOrder);

        await dbContext.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{id:int}/buttons")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddButton(string slug, int id, string text, string url, string styleClass, CancellationToken cancellationToken)
    {
        var section = await FindSectionAsync(slug, id, cancellationToken);
        if (section is null)
        {
            return NotFound();
        }

        var nextSortOrder = section.Buttons.Count > 0 ? section.Buttons.Max(button => button.SortOrder) + 10 : 10;
        section.Buttons.Add(new PageSectionButton
        {
            Text = string.IsNullOrWhiteSpace(text) ? "Кнопка" : text.Trim(),
            Url = string.IsNullOrWhiteSpace(url) ? "/" : url.Trim(),
            StyleClass = NormalizeButtonStyle(styleClass),
            SortOrder = nextSortOrder
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Кнопка добавлена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/buttons/{buttonId:int}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateButton(string slug, int sectionId, int buttonId, string text, string url, string styleClass, CancellationToken cancellationToken)
    {
        var button = await dbContext.PageSectionButtons
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == buttonId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);

        if (button is null)
        {
            return NotFound();
        }

        button.Text = string.IsNullOrWhiteSpace(text) ? "Кнопка" : text.Trim();
        button.Url = string.IsNullOrWhiteSpace(url) ? "/" : url.Trim();
        button.StyleClass = NormalizeButtonStyle(styleClass);

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Кнопка обновлена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("{sectionId:int}/buttons/{buttonId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteButton(string slug, int sectionId, int buttonId, CancellationToken cancellationToken)
    {
        var button = await dbContext.PageSectionButtons
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == buttonId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);

        if (button is null)
        {
            return NotFound();
        }

        dbContext.PageSectionButtons.Remove(button);
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["AdminMessage"] = "Кнопка удалена.";
        return RedirectToAction(nameof(Index), new { slug });
    }

    private async Task<PageSection?> FindSectionAsync(string slug, int id, CancellationToken cancellationToken)
    {
        return await dbContext.PageSections
            .Include(section => section.SitePage)
            .Include(section => section.Buttons)
            .Include(section => section.Cards)
            .Include(section => section.Testimonials)
            .Include(section => section.PortfolioImages)
            .SingleOrDefaultAsync(section => section.Id == id && section.SitePage != null && section.SitePage.Slug == slug, cancellationToken);
    }

    private async Task<PageSectionTestimonial?> FindTestimonialAsync(string slug, int sectionId, int testimonialId, CancellationToken cancellationToken)
    {
        return await dbContext.PageSectionTestimonials
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == testimonialId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);
    }

    private async Task<PageSectionPortfolioImage?> FindPortfolioImageAsync(string slug, int sectionId, int imageId, CancellationToken cancellationToken)
    {
        return await dbContext.PageSectionPortfolioImages
            .Include(item => item.PageSection)
                .ThenInclude(section => section!.SitePage)
            .SingleOrDefaultAsync(item => item.Id == imageId && item.PageSectionId == sectionId && item.PageSection != null && item.PageSection.SitePage != null && item.PageSection.SitePage.Slug == slug, cancellationToken);
    }

    private static string? TrimOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeTemplate(string value)
    {
        return TemplateOptions.Any(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            ? value
            : TextImageTemplate;
    }

    private static string NormalizeLayout(string? value)
    {
        return LayoutOptions.Any(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            ? value!
            : "image-right";
    }

    private static string? NormalizeOptionalUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
    }

    private static string NormalizeBackground(string value)
    {
        return BackgroundOptions.Any(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            ? value
            : "surface-cream";
    }

    private static string NormalizeTextColor(string value)
    {
        return TextColorOptions.Any(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            ? value
            : "text-accent-charcoal";
    }

    private static string NormalizeButtonStyle(string value)
    {
        return ButtonStyleOptions.Any(option => string.Equals(option.Value, value, StringComparison.OrdinalIgnoreCase))
            ? value
            : "button--primary";
    }

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> LayoutOptions =
    [
        new AdminSelectOptionViewModel { Value = "image-right", Label = "Текст слева, изображение справа" },
        new AdminSelectOptionViewModel { Value = "image-left", Label = "Изображение слева, текст справа" },
        new AdminSelectOptionViewModel { Value = "image-top", Label = "Изображение сверху, текст снизу" },
        new AdminSelectOptionViewModel { Value = "image-bottom", Label = "Текст сверху, изображение снизу" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> TemplateOptions =
    [
        new AdminSelectOptionViewModel { Value = TextImageTemplate, Label = "Шаблон 1: текст + изображение + кнопки" },
        new AdminSelectOptionViewModel { Value = CardsGridTemplate, Label = "Шаблон 2: заголовок + карточки" },
        new AdminSelectOptionViewModel { Value = TestimonialsTemplate, Label = "Шаблон 3: отзывы с фото" },
        new AdminSelectOptionViewModel { Value = MapTemplate, Label = "Шаблон 4: карта Яндекс" },
        new AdminSelectOptionViewModel { Value = PortfolioTemplate, Label = "Шаблон 5: портфолио с каруселью" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> BackgroundOptions =
    [
        new AdminSelectOptionViewModel { Value = "surface-cream", Label = "Кремовый" },
        new AdminSelectOptionViewModel { Value = "surface-white", Label = "Белый" },
        new AdminSelectOptionViewModel { Value = "surface-warm", Label = "Теплый светлый" },
        new AdminSelectOptionViewModel { Value = "surface-sand", Label = "Песочный" },
        new AdminSelectOptionViewModel { Value = "surface-gold-soft", Label = "Мягкий золотой" },
        new AdminSelectOptionViewModel { Value = "surface-sage-soft", Label = "Мягкий шалфей" },
        new AdminSelectOptionViewModel { Value = "surface-terracotta-soft", Label = "Мягкая терракота" },
        new AdminSelectOptionViewModel { Value = "surface-stone", Label = "Каменный" },
        new AdminSelectOptionViewModel { Value = "surface-mist", Label = "Светлый туман" },
        new AdminSelectOptionViewModel { Value = "surface-charcoal", Label = "Темный графит" },
        new AdminSelectOptionViewModel { Value = "surface-forest", Label = "Темный зеленый" },
        new AdminSelectOptionViewModel { Value = "surface-ink", Label = "Черный" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> TextColorOptions =
    [
        new AdminSelectOptionViewModel { Value = "text-accent-gold", Label = "Золотой" },
        new AdminSelectOptionViewModel { Value = "text-accent-charcoal", Label = "Темный графит" },
        new AdminSelectOptionViewModel { Value = "text-accent-terracotta", Label = "Терракотовый" },
        new AdminSelectOptionViewModel { Value = "text-accent-sage", Label = "Шалфейный" },
        new AdminSelectOptionViewModel { Value = "text-accent-cream", Label = "Светлый кремовый" },
        new AdminSelectOptionViewModel { Value = "text-accent-brass", Label = "Латунный" },
        new AdminSelectOptionViewModel { Value = "text-accent-copper", Label = "Медный" },
        new AdminSelectOptionViewModel { Value = "text-accent-olive", Label = "Оливковый" },
        new AdminSelectOptionViewModel { Value = "text-accent-forest", Label = "Темно-зеленый" },
        new AdminSelectOptionViewModel { Value = "text-accent-steel", Label = "Стальной" },
        new AdminSelectOptionViewModel { Value = "text-accent-warm-gray", Label = "Теплый серый" },
        new AdminSelectOptionViewModel { Value = "text-accent-ruby", Label = "Рубиновый" }
    ];

    private static readonly IReadOnlyList<AdminSelectOptionViewModel> ButtonStyleOptions =
    [
        new AdminSelectOptionViewModel { Value = "button--primary", Label = "Темная" },
        new AdminSelectOptionViewModel { Value = "button--secondary", Label = "Светлая с рамкой" },
        new AdminSelectOptionViewModel { Value = "button--gold", Label = "Золотая" },
        new AdminSelectOptionViewModel { Value = "button--sage", Label = "Шалфейная" },
        new AdminSelectOptionViewModel { Value = "button--cream", Label = "Кремовая" },
        new AdminSelectOptionViewModel { Value = "button--charcoal-outline", Label = "Графитовая рамка" },
        new AdminSelectOptionViewModel { Value = "button--brass", Label = "Латунная" },
        new AdminSelectOptionViewModel { Value = "button--copper", Label = "Медная" },
        new AdminSelectOptionViewModel { Value = "button--olive", Label = "Оливковая" },
        new AdminSelectOptionViewModel { Value = "button--forest", Label = "Темно-зеленая" },
        new AdminSelectOptionViewModel { Value = "button--steel", Label = "Стальная" },
        new AdminSelectOptionViewModel { Value = "button--ruby", Label = "Рубиновая" }
    ];
}
