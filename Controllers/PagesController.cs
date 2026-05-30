using ComfortRooms.Services;
using ComfortRooms.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ComfortRooms.Controllers;

public sealed class PagesController(ILeadRequestService leadRequestService) : Controller
{
    [HttpGet("sotrudnichestvo/opt")]
    public IActionResult Wholesale()
    {
        return View("CooperationAudience", new CooperationAudiencePageViewModel
        {
            Title = "Оптовым партнерам",
            Eyebrow = "Сотрудничество",
            Description = "Раздел для оптовых партнеров, комплектации объектов и регулярных поставок светильников Comfort Rooms.",
            Accent = "Оптовая работа строится на понятных условиях, стабильной коммуникации и аккуратной поддержке каждого заказа.",
            ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/194cf7431-b515-4024-82dd-ec4c7a499814.png",
            ImageAlt = "Люстра для оптового сотрудничества",
            Cards =
            [
                new CooperationAudienceCardViewModel { Number = "01", Title = "Поставки", Description = "Формируем предложение под формат партнера, объемы и специфику объектов." },
                new CooperationAudienceCardViewModel { Number = "02", Title = "Проектные заказы", Description = "Помогаем с нестандартными запросами, комплектацией и изделиями под интерьер." },
                new CooperationAudienceCardViewModel { Number = "03", Title = "Документы и этапы", Description = "Фиксируем договоренности, сроки и состав заказа, чтобы работа была предсказуемой." }
            ],
            Steps =
            [
                "Обсуждаем формат сотрудничества, объемы и типовые задачи партнера.",
                "Готовим предложение, условия и набор материалов для старта.",
                "Сопровождаем заказы, поставки и индивидуальные запросы по проектам."
            ]
        });
    }

    [HttpGet("sotrudnichestvo/roznitsa")]
    public IActionResult Retail()
    {
        return View("CooperationAudience", new CooperationAudiencePageViewModel
        {
            Title = "Розничным клиентам",
            Eyebrow = "Сотрудничество",
            Description = "Помогаем частным клиентам и розничным покупателям подобрать светильники для интерьера или заказать изделие под конкретную задачу.",
            Accent = "Розничная работа строится вокруг понятного выбора, аккуратной консультации и светильника, который подходит именно вашему пространству.",
            ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/108b0bc3f-abda-4b9a-a3c7-de67ba54c337.png",
            ImageAlt = "Схема люстры для розничного клиента",
            Cards =
            [
                new CooperationAudienceCardViewModel { Number = "01", Title = "Подбор", Description = "Помогаем выбрать форму, размер, отделку и световой сценарий под интерьер." },
                new CooperationAudienceCardViewModel { Number = "02", Title = "Индивидуальный заказ", Description = "Если готовые модели не подходят, можно рассчитать светильник по референсу или эскизу." },
                new CooperationAudienceCardViewModel { Number = "03", Title = "Сопровождение", Description = "Подсказываем по деталям заказа, доставке, монтажу и дальнейшей эксплуатации." }
            ],
            Steps =
            [
                "Вы описываете интерьер, размеры, пожелания и бюджет.",
                "Мы предлагаем подходящие варианты или путь индивидуального изготовления.",
                "После согласования помогаем довести заказ до готового светильника."
            ]
        });
    }

    [HttpGet("sotrudnichestvo/dizayneram")]
    public IActionResult Designers()
    {
        return View("CooperationAudience", new CooperationAudiencePageViewModel
        {
            Title = "Дизайнерам",
            Eyebrow = "Сотрудничество",
            Description = "Помогаем дизайнерам интерьеров решать задачи со светом: от подбора готовых моделей до изготовления авторских светильников под конкретный проект.",
            Accent = "Мы бережно относимся к авторской идее и помогаем довести ее до технически реализуемого изделия.",
            ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/111275a01-589c-4089-96be-fc07fb47357d.png",
            ImageAlt = "Люстра для дизайнерского интерьера",
            Cards =
            [
                new CooperationAudienceCardViewModel { Number = "01", Title = "Индивидуальные изделия", Description = "Работаем по эскизам, чертежам, референсам и визуализациям проекта." },
                new CooperationAudienceCardViewModel { Number = "02", Title = "Проектная поддержка", Description = "Помогаем уточнить размеры, материалы, покрытие, крепление и сценарии монтажа." },
                new CooperationAudienceCardViewModel { Number = "03", Title = "Комфортная коммуникация", Description = "Фиксируем этапы, сроки и решения, чтобы дизайнеру было проще вести клиента." }
            ],
            Steps =
            [
                "Получаем вводные по объекту, референсы и требования к светильнику.",
                "Предлагаем конструктивное решение, материалы и ориентир по стоимости.",
                "Согласовываем детали, запускаем производство и сопровождаем проект до результата."
            ]
        });
    }

    [HttpGet("sotrudnichestvo/magazinam")]
    public IActionResult Shops()
    {
        return View("CooperationAudience", new CooperationAudiencePageViewModel
        {
            Title = "Магазинам",
            Eyebrow = "Сотрудничество",
            Description = "Развиваем партнерство с магазинами и салонами света: помогаем формировать ассортимент, работать с запросами клиентов и поддерживать продажи.",
            Accent = "Для розницы важны понятные условия, стабильная коммуникация и визуально сильный продукт.",
            ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/109d82bb8-7d01-4b67-9aa3-2b000e635158.png",
            ImageAlt = "Премиальная люстра для салона света",
            Cards =
            [
                new CooperationAudienceCardViewModel { Number = "01", Title = "Ассортимент", Description = "Помогаем выстроить предложение под аудиторию магазина или салона." },
                new CooperationAudienceCardViewModel { Number = "02", Title = "Материалы для продаж", Description = "Готовим структуру для изображений, описаний и демонстрационных блоков." },
                new CooperationAudienceCardViewModel { Number = "03", Title = "Работа с запросами", Description = "Поддерживаем нестандартные обращения и изделия под конкретного клиента." }
            ],
            Steps =
            [
                "Обсуждаем формат магазина, аудиторию и желаемое направление ассортимента.",
                "Согласовываем условия, страницы, изображения и стартовый набор материалов.",
                "Поддерживаем обращения клиентов и расширение линейки по мере роста продаж."
            ]
        });
    }

    [HttpGet("sotrudnichestvo/internet-magazinam")]
    public IActionResult Ecommerce()
    {
        return View("CooperationAudience", new CooperationAudiencePageViewModel
        {
            Title = "Интернет-магазинам",
            Eyebrow = "E-commerce",
            Description = "Готовим основу для онлайн-продаж: карточки, изображения, описания, категории и понятную работу с заявками по индивидуальным изделиям.",
            Accent = "Онлайн-витрина должна быть не только красивой, но и удобной для покупки, поиска и консультации.",
            ImageUrl = "https://image.qwenlm.ai/public_source/0292984e-e4f3-4e94-9aaf-0ce408d282fe/1ab29de9e-b6b6-46fd-b809-83c08fdbf763.png",
            ImageAlt = "Современная люстра для e-commerce витрины",
            Cards =
            [
                new CooperationAudienceCardViewModel { Number = "01", Title = "Контент", Description = "Структурируем изображения, названия, описания и особенности изделий." },
                new CooperationAudienceCardViewModel { Number = "02", Title = "Каталог", Description = "Закладываем понятную логику категорий и будущих карточек товаров." },
                new CooperationAudienceCardViewModel { Number = "03", Title = "Заявки", Description = "Формы уже сохраняют обращения в админке, что удобно для обработки заказов." }
            ],
            Steps =
            [
                "Определяем формат интернет-магазина, каталог и требования к карточкам.",
                "Готовим визуальный и текстовый контент для понятной онлайн-витрины.",
                "Подключаем заявки и постепенно расширяем функциональность под продажи."
            ]
        });
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
