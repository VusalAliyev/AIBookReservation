using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;
using System.Security.Claims;

namespace BookReservation.Controllers;

[Authorize(Roles = "Student")]
public class RequestsController : Controller
{
    private readonly IRequestService _requestService;
    
    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }
    
    public async Task<IActionResult> MyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var requests = await _requestService.GetStudentRequestsAsync(userId);
        return View(requests);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var request = await _requestService.CreateRequestAsync(bookId, userId);
        
        if (request == null)
        {
            TempData["Error"] = "Unable to create request. You may already have a pending request or an active loan for this book.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }
        
        TempData["Success"] = "Borrow request submitted successfully!";
        return RedirectToAction("MyRequests");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var success = await _requestService.CancelRequestAsync(id, userId);
        
        if (success)
        {
            TempData["Success"] = "Request cancelled successfully!";
        }
        else
        {
            TempData["Error"] = "Unable to cancel request.";
        }
        
        return RedirectToAction("MyRequests");
    }
}
