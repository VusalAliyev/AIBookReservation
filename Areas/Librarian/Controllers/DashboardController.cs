using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class DashboardController : Controller
{
    private readonly IBookService _bookService;
    private readonly IRequestService _requestService;
    private readonly ILoanService _loanService;
    
    public DashboardController(
        IBookService bookService,
        IRequestService requestService,
        ILoanService loanService)
    {
        _bookService = bookService;
        _requestService = requestService;
        _loanService = loanService;
    }
    
    public async Task<IActionResult> Index()
    {
        var allBooks = await _bookService.GetAllBooksAsync();
        var pendingRequests = await _requestService.GetPendingRequestsAsync();
        var activeLoans = await _loanService.GetActiveLoansAsync();
        var overdueLoans = await _loanService.GetOverdueLoansAsync();
        
        ViewBag.TotalBooks = allBooks.Count();
        ViewBag.AvailableBooks = allBooks.Count(b => b.AvailableCount > 0);
        ViewBag.PendingRequests = pendingRequests.Count();
        ViewBag.ActiveLoans = activeLoans.Count();
        ViewBag.OverdueLoans = overdueLoans.Count();
        
        return View();
    }
}
