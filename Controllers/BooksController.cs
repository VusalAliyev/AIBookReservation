using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReservation.Services;
using BookReservation.Models;
using BookReservation.ViewModels;

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

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int pageNumber = 1)
    {
        const int pageSize = 12;

        var booksQuery = _bookService.GetBooksQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            booksQuery = booksQuery.Where(b => 
                b.Title.Contains(searchTerm) || 
                b.Author.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            booksQuery = booksQuery.Where(b => b.CategoryId == categoryId.Value);
        }

        booksQuery = booksQuery.Include(b => b.Category).OrderBy(b => b.Title);

        var paginatedBooks = await PaginatedList<Book>.CreateAsync(booksQuery, pageNumber, pageSize);
        var categories = await _categoryService.GetAllCategoriesAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = categories;

        return View(paginatedBooks);
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
