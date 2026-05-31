using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ComfortRooms.Controllers;

[Route("sotrudnichestvo")]
public class CooperationController(IPageContentService pageContentService, ILeadRequestService leadRequestService) : Controller
{
    [HttpGet("na-zakaz")]
    public async Task<IActionResult> CustomOrder(CancellationToken cancellationToken)
    {
        var model = await pageContentService.GetCustomOrderPageAsync(cancellationToken);
        return View(model);
    }

    [HttpPost("na-zakaz")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CustomOrderLead([Bind(Prefix = "LeadRequest")] LeadRequestFormViewModel leadRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var model = await pageContentService.GetCustomOrderPageAsync(cancellationToken);
            return View("CustomOrder", new CustomOrderPageViewModel
            {
                HeroTitle = model.HeroTitle,
                HeroDescription = model.HeroDescription,
                GalleryImages = model.GalleryImages,
                LeadRequest = leadRequest
            });
        }

        await leadRequestService.CreateAsync(leadRequest, PageSlugs.CustomOrder, cancellationToken);
        TempData["LeadRequestSuccess"] = "Заявка отправлена. Мы свяжемся с вами в ближайшее время.";

        return Redirect("/sotrudnichestvo/na-zakaz#contact");
    }
}
