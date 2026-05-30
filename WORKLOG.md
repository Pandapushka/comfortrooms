# Журнал разработки Comfort Rooms

Этот файл нужен, чтобы быстро восстановить контекст после перезагрузки или новой сессии.

## Основные правила проекта

- Проект называется `ComfortRooms`, публичное название — `Comfort Rooms`.
- Используем только .NET 8, целевой фреймворк `net8.0`.
- Git-действия, коммиты, пуши, merge/rebase — только после явного разрешения пользователя.
- Язык общения, комментариев и интерфейса — русский.
- Готовая HTML-страница «Изготовление люстр под заказ» является главным визуальным ориентиром.
- Карусель изображений на странице «на заказ» должна управляться через админку.
- Обычные пользователи не регистрируются и не входят на сайт. Авторизация нужна только администратору.

## Что уже сделано

- Установлен .NET SDK `8.0.421`.
- Добавлен `global.json`, закрепляющий SDK `8.0.421`.
- Создан ASP.NET Core MVC-проект `ComfortRooms`.
- Проект переведен на `net8.0`.
- Исправлены API шаблона .NET 10 на совместимые с ASP.NET Core 8.
- Добавлена Razor-страница `/sotrudnichestvo/na-zakaz` на основе готового HTML.
- Карусель на странице «на заказ» выводится через `CustomOrderPageViewModel.GalleryImages`.
- Добавлены EF Core SQLite-пакеты версии `8.0.17`.
- Добавлены сущности:
  - `AdminUser`
  - `SitePage`
  - `PageImage`
  - `LeadRequest`
- Добавлен `ComfortRoomsDbContext`.
- Добавлены сервисы:
  - `IAdminPasswordHasher` / `Pbkdf2AdminPasswordHasher`
  - `IImageStorageService` / `LocalImageStorageService`
  - `IPageContentService` / `PageContentService`
- Добавлен `DatabaseInitializer` с начальным заполнением страниц и галереи.
- В `Program.cs` подключены:
  - SQLite
  - cookie-аутентификация администратора
  - сервисы проекта
  - route для Areas
- Добавлены контроллеры админки:
  - `Areas/Admin/Controllers/AccountController.cs`
  - `Areas/Admin/Controllers/DashboardController.cs`
  - `Areas/Admin/Controllers/ImagesController.cs`
- Добавлен файл журнала `WORKLOG.md`.
- Добавлены Razor-представления админки:
  - `Areas/Admin/Views/Shared/_AdminLayout.cshtml`
  - `Areas/Admin/Views/Account/Login.cshtml`
  - `Areas/Admin/Views/Dashboard/Index.cshtml`
  - `Areas/Admin/Views/Images/Index.cshtml`
  - `Areas/Admin/Views/_ViewImports.cshtml`
- Добавлены стили админки в `wwwroot/css/site.css`.
- Пароль администратора не хранится в `appsettings*.json`; первичная учетная запись создается только если заданы `AdminSeed__Login` и `AdminSeed__Password`.
- Добавлена безопасная страница первичной настройки `/admin/setup`, доступная только пока в БД нет администратора.
- Добавлены:
  - `Areas/Admin/Controllers/SetupController.cs`
  - `Areas/Admin/Views/Setup/Index.cshtml`
  - `ViewModels/AdminSetupViewModel.cs`
- Сборка после добавления админки успешна: `0` ошибок, `0` предупреждений.
- Проверены GET-маршруты:
  - `/sotrudnichestvo/na-zakaz` — `200`
  - `/admin/setup` — `200`
  - `/admin/login` — `200`
  - `/admin` без авторизации — `302` на `/admin/login`
- Форма заявки на странице `/sotrudnichestvo/na-zakaz` переведена с JS-имитации на реальный POST.
- Добавлены:
  - `ViewModels/LeadRequestFormViewModel.cs`
  - `ViewModels/AdminLeadRequestViewModel.cs`
  - `Services/ILeadRequestService.cs`
  - `Services/LeadRequestService.cs`
  - `Areas/Admin/Controllers/LeadsController.cs`
  - `Areas/Admin/Views/Leads/Index.cshtml`
- Заявки сохраняются в таблицу `LeadRequests`.
- В админ-layout добавлена ссылка «Заявки».
- Проверен POST формы с anti-forgery token: сервер вернул `302` на `/sotrudnichestvo/na-zakaz#contact`.
- Проверена запись в SQLite: тестовая заявка появилась в `LeadRequests`.
- Собственные inline CSS/JS страницы `/sotrudnichestvo/na-zakaz` вынесены в локальные файлы:
  - `wwwroot/css/custom-order.css`
  - `wwwroot/js/custom-order.js`
- Убрано неиспользуемое модальное окно успешной заявки после перехода формы на реальный POST.
- Публичный layout получил основную навигацию:
  - Главная
  - Сотрудничество с выпадающими пунктами
  - О компании
  - Контакты
  - внешний «Интернет-магазин»
  - Админка
- Добавлен `PagesController` и простая общая view-заглушка `Views/Pages/SimplePage.cshtml` для будущих контентных страниц.
- После обновления ассетов и публичной навигации сборка успешна: `0` ошибок, `0` предупреждений.
- Проверены GET-маршруты:
  - `/` — `200`
  - `/Pages/Designers` — `200`
  - `/Pages/Shops` — `200`
  - `/Pages/Contacts` — `200`
- Добавлены красивые публичные маршруты:
  - `/o-kompanii`
  - `/kontakty`
  - `/sotrudnichestvo/dizayneram`
  - `/sotrudnichestvo/magazinam`
  - `/sotrudnichestvo/internet-magazinam`
- Публичное меню обновлено на эти маршруты.
- Страницы-заглушки получили более содержательную структуру с hero-блоком и тремя карточками.
- Проверены новые GET-маршруты:
  - `/o-kompanii` — `200`
  - `/kontakty` — `200`
  - `/sotrudnichestvo/dizayneram` — `200`
  - `/sotrudnichestvo/magazinam` — `200`
  - `/sotrudnichestvo/internet-magazinam` — `200`
- Главная страница перестала быть временной:
  - добавлен hero с изображением и CTA;
  - добавлен блок направлений;
  - добавлен блок подхода/преимуществ;
  - добавлен CTA на заявку;
  - стили добавлены в `wwwroot/css/site.css`.
- После обновления главной сборка успешна: `0` ошибок, `0` предупреждений.
- Проверен GET `/` — `200`.
- Страница `/kontakty` заменена с заглушки на полноценную контактную страницу:
  - добавлены контакты, адрес, информационные карточки и форма заявки;
  - форма сохраняет заявки через `LeadRequestService` со slug `kontakty`;
  - добавлен `ContactsPageViewModel`;
  - добавлена view `Views/Pages/Contacts.cshtml`;
  - стили добавлены в `wwwroot/css/site.css`.
- После обновления контактов сборка успешна: `0` ошибок, `0` предупреждений.
- Проверен GET `/kontakty` — `200`.
- Проверен POST формы `/kontakty` с anti-forgery token — `302` на `/kontakty#contact-form`.
- Проверена запись в SQLite: заявка появилась в `LeadRequests` со slug `kontakty`.
- В админке изображений добавлено редактирование названия и alt-текста существующих изображений.
- `IImageStorageService` получил метод `DeleteImageAsync`.
- `LocalImageStorageService` теперь удаляет локальный файл из `wwwroot/uploads` при удалении изображения из админки.
- После обновления админки изображений сборка успешна: `0` ошибок, `0` предупреждений.

## Где остановились

Сервер запущен на `http://localhost:5000`.

## Следующие шаги

1. Создать администратора через `/admin/setup`.
2. После входа проверить `/admin/pages/sotrudnichestvo-na-zakaz/images`.
3. После входа проверить `/admin/leads`.
4. Полностью заменить Tailwind CDN и внешние шрифты/Lucide на локальные ассеты или сборку фронтенда.
5. Наполнить заглушки страниц реальными секциями в стиле страницы «на заказ».
6. Добавить просмотр/редактирование текстового контента страниц через админку.
7. Добавить миграции EF Core вместо `EnsureCreated`, чтобы база развивалась безопасно.
8. Добавить замену изображения без удаления карточки.
