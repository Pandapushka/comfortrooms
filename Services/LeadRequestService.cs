using ComfortRooms.Data;
using ComfortRooms.Models;
using ComfortRooms.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ComfortRooms.Services;

public sealed class LeadRequestService(ComfortRoomsDbContext dbContext) : ILeadRequestService
{
    public async Task CreateAsync(LeadRequestFormViewModel form, string sourcePageSlug, CancellationToken cancellationToken)
    {
        dbContext.LeadRequests.Add(new LeadRequest
        {
            Name = form.Name.Trim(),
            Phone = form.Phone.Trim(),
            Message = string.IsNullOrWhiteSpace(form.Message) ? null : form.Message.Trim(),
            SourcePageSlug = sourcePageSlug
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminLeadRequestViewModel>> GetLatestAsync(CancellationToken cancellationToken)
    {
        return await dbContext.LeadRequests
            .AsNoTracking()
            .OrderByDescending(request => request.CreatedAtUtc)
            .Select(request => new AdminLeadRequestViewModel
            {
                Id = request.Id,
                Name = request.Name,
                Phone = request.Phone,
                Message = request.Message,
                SourcePageSlug = request.SourcePageSlug,
                CreatedAtUtc = request.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
