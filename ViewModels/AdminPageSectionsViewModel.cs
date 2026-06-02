namespace ComfortRooms.ViewModels;

public sealed class AdminPageSectionsViewModel
{
    public required string PageSlug { get; init; }

    public required string PageTitle { get; init; }

    public IReadOnlyList<AdminPageSectionItemViewModel> Sections { get; init; } = [];

    public IReadOnlyList<AdminSelectOptionViewModel> TemplateOptions { get; init; } = [];

    public IReadOnlyList<AdminSelectOptionViewModel> LayoutOptions { get; init; } = [];

    public IReadOnlyList<AdminSelectOptionViewModel> BackgroundOptions { get; init; } = [];

    public IReadOnlyList<AdminSelectOptionViewModel> TextColorOptions { get; init; } = [];

    public IReadOnlyList<AdminSelectOptionViewModel> ButtonStyleOptions { get; init; } = [];
}
