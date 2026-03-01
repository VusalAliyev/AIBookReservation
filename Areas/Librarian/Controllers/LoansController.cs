using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    
    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }
    
    public async Task<IActionResult> Index()
    {
        var loans = await _loanService.GetAllLoansAsync();
        return View(loans);
    }
    
    public async Task<IActionResult> Active()
    {
        var loans = await _loanService.GetActiveLoansAsync();
        return View(loans);
    }
    
    public async Task<IActionResult> Overdue()
    {
        var loans = await _loanService.GetOverdueLoansAsync();
        return View(loans);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var success = await _loanService.ReturnLoanAsync(id);
        
        if (success)
        {
            TempData["Success"] = "Loan marked as returned!";
        }
        else
        {
            TempData["Error"] = "Unable to return loan.";
        }
        
        return RedirectToAction(nameof(Active));
    }
}
