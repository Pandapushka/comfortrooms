using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ComfortRooms.Controllers;

public sealed class PagesController(ILeadRequestService leadRequestService) : Controller
{
    [HttpGet("sotrudnichestvo/dizayneram")]
    public IActionResult Designers()
    {
        ViewData["Title"] = "Дизайнерам";
        ViewData["Eyebrow"] = "Сотрудничество";
        ViewData["Description"] = "Партнерская страница для дизайнеров интерьеров: условия, проекты, индивидуальные изделия и сопровождение заказов.";
        ViewData["Accent"] = "Помогаем воплощать сложные световые сценарии в частных и коммерческих интерьерах.";
        return View("SimplePage");
    }

    [HttpGet("sotrudnichestvo/magazinam")]
    public IActionResult Shops()
    {
        ViewData["Title"] = "Магазинам";
        ViewData["Eyebrow"] = "Сотрудничество";
        ViewData["Description"] = "Раздел для розничных магазинов и салонов света: ассортимент, поставки, демонстрационные материалы и условия работы.";
        ViewData["Accent"] = "Создаем устойчивые поставки и понятную витрину для салонов света.";
        return View("SimplePage");
    }

    [HttpGet("sotrudnichestvo/internet-magazinam")]
    public IActionResult Ecommerce()
    {
        ViewData["Title"] = "Интернет-магазинам";
        ViewData["Eyebrow"] = "Сотрудничество";
        ViewData["Description"] = "Страница для e-commerce партнеров: контент, карточки товаров, логистика и поддержка продаж.";
        ViewData["Accent"] = "Готовим структуру для карточек, изображений и дальнейшей интеграции с онлайн-продажами.";
        return View("SimplePage");
    }

    [HttpGet("kontakty")]
    public IActionResult Contacts()
    {
        return View(new ContactsPageViewModel());
    }

    [HttpPost("kontakty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contacts([Bind(Prefix = "LeadRequest")] LeadRequestFormViewModel leadRequest, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(new ContactsPageViewModel
            {
                LeadRequest = leadRequest
            });
        }

        await leadRequestService.CreateAsync(leadRequest, PageSlugs.Contacts, cancellationToken);
        TempData["LeadRequestSuccess"] = "Заявка отправлена. Мы свяжемся с вами в ближайшее время.";

        return Redirect("/kontakty#contact-form");
    }
}
