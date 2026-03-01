using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Models;
using Microsoft.EntityFrameworkCore;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class StudentsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public StudentsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");
        return View(students);
    }
}
