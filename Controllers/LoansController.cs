using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;
using System.Security.Claims;

namespace BookReservation.Controllers;

[Authorize(Roles = "Student")]
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    
    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }
    
    public async Task<IActionResult> MyActiveLoans()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var loans = await _loanService.GetStudentActiveLoansAsync(userId);
        return View(loans);
    }
    
    public async Task<IActionResult> MyHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var loans = await _loanService.GetStudentLoanHistoryAsync(userId);
        return View(loans);
    }
}
