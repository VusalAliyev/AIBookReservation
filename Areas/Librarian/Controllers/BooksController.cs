using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;
using BookReservation.Models;
using BookReservation.ViewModels;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;
    private readonly ICategoryService _categoryService;
    
    public BooksController(IBookService bookService, ICategoryService categoryService)
    {
        _bookService = bookService;
        _categoryService = categoryService;
    }
    
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }
    
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        if (model.AvailableCount > model.TotalCount)
        {
            ModelState.AddModelError("AvailableCount", "Available count cannot exceed total count.");
        }
        
        if (ModelState.IsValid)
        {
            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                ISBN = model.ISBN,
                CategoryId = model.CategoryId,
                TotalCount = model.TotalCount,
                AvailableCount = model.AvailableCount
            };
            
            await _bookService.CreateBookAsync(book);
            TempData["Success"] = "Book created successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View(model);
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        
        var model = new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            ISBN = book.ISBN,
            CategoryId = book.CategoryId,
            TotalCount = book.TotalCount,
            AvailableCount = book.AvailableCount
        };
        
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookViewModel model)
    {
        if (model.AvailableCount > model.TotalCount)
        {
            ModelState.AddModelError("AvailableCount", "Available count cannot exceed total count.");
        }
        
        if (ModelState.IsValid)
        {
            var book = new Book
            {
                Id = model.Id,
                Title = model.Title,
                Author = model.Author,
                Description = model.Description,
                ISBN = model.ISBN,
                CategoryId = model.CategoryId,
                TotalCount = model.TotalCount,
                AvailableCount = model.AvailableCount
            };
            
            var result = await _bookService.UpdateBookAsync(book);
            if (result == null)
            {
                return NotFound();
            }
            
            TempData["Success"] = "Book updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _bookService.DeleteBookAsync(id);
        
        if (success)
        {
            TempData["Success"] = "Book deleted successfully!";
        }
        else
        {
            TempData["Error"] = "Unable to delete book. It may have active loans.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}
