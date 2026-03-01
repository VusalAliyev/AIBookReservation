using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookReservation.Services;
using BookReservation.Models;
using BookReservation.ViewModels;

namespace BookReservation.Areas.Librarian.Controllers;

[Area("Librarian")]
[Authorize(Roles = "Librarian")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;
    
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return View(categories);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };
            
            await _categoryService.CreateCategoryAsync(category);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(model);
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        
        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = new Category
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };
            
            var result = await _categoryService.UpdateCategoryAsync(category);
            if (result == null)
            {
                return NotFound();
            }
            
            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        
        if (success)
        {
            TempData["Success"] = "Category deleted successfully!";
        }
        else
        {
            TempData["Error"] = "Unable to delete the Uncategorized category.";
        }
        
        return RedirectToAction(nameof(Index));
    }
}
