using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;
using System.Security.Claims;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class RequestsController : Controller
{
    private readonly IRequestService _requestService;
    
    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }
    
    public async Task<IActionResult> Index()
    {
        var requests = await _requestService.GetAllRequestsAsync();
        return View(requests);
    }
    
    public async Task<IActionResult> Pending()
    {
        var requests = await _requestService.GetPendingRequestsAsync();
        return View(requests);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int id)
    {
        var librarianId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _requestService.AcceptRequestAsync(id, librarianId);
        
        if (success)
        {
            TempData["Success"] = "Request accepted and loan created!";
        }
        else
        {
            TempData["Error"] = "Unable to accept request. Check book availability and student eligibility.";
        }
        
        return RedirectToAction(nameof(Pending));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deny(int id)
    {
        var librarianId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _requestService.DenyRequestAsync(id, librarianId);
        
        if (success)
        {
            TempData["Success"] = "Request denied!";
        }
        else
        {
            TempData["Error"] = "Unable to deny request.";
        }
        
        return RedirectToAction(nameof(Pending));
    }
}
