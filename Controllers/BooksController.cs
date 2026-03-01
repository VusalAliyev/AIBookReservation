using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;

namespace BookReservation.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;
    private readonly IRequestService _requestService;
    
    public BooksController(
        IBookService bookService,
        ICategoryService categoryService,
        IRequestService requestService)
    {
        _bookService = bookService;
        _categoryService = categoryService;
        _requestService = requestService;
    }
    
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId)
    {
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CategoryId = categoryId;
        
        var books = await _bookService.SearchBooksAsync(searchTerm, categoryId);
        return View(books);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                ViewBag.CanRequest = await _requestService.CanStudentCreateRequestAsync(userId, id);
            }
        }
        
        return View(book);
    }
}
