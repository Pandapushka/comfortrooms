using ComfortRooms.ViewModels;

namespace ComfortRooms.Services;

public interface ILeadRequestService
{
    Task CreateAsync(LeadRequestFormViewModel form, string sourcePageSlug, CancellationToken cancellationToken);

    Task<IReadOnlyList<AdminLeadRequestViewModel>> GetLatestAsync(CancellationToken cancellationToken);
}
