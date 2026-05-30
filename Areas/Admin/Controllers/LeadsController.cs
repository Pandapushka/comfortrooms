using ComfortRooms.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComfortRooms.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("admin/leads")]
public sealed class LeadsController(ILeadRequestService leadRequestService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var leads = await leadRequestService.GetLatestAsync(cancellationToken);
        return View(leads);
    }
}
