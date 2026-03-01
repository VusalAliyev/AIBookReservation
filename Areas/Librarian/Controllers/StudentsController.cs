using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Models;
using BookReservation.ViewModels;
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

    public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1)
    {
        const int pageSize = 20;

        var students = await _userManager.GetUsersInRoleAsync("Student");

        IEnumerable<ApplicationUser> filteredStudents = students;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredStudents = students.Where(s => 
                s.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (s.Email != null && s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (s.UserName != null && s.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        var orderedStudents = filteredStudents.OrderBy(s => s.FullName);
        var paginatedStudents = PaginatedList<ApplicationUser>.Create(orderedStudents, pageNumber, pageSize);

        ViewBag.SearchTerm = searchTerm;

        return View(paginatedStudents);
    }
}
