using Microsoft.EntityFrameworkCore;
using BookReservation.Data;
using BookReservation.Models;

namespace BookReservation.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    
    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
    }
    
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }
    
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        return category;
    }
    
    public async Task<Category?> UpdateCategoryAsync(Category category)
    {
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing == null)
            return null;
        
        existing.Name = category.Name;
        existing.Description = category.Description;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return existing;
    }
    
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;
        
        if (category.Name == "Uncategorized")
            return false;
        
        var uncategorized = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Uncategorized");
        
        if (uncategorized == null)
            return false;
        
        var booksInCategory = await _context.Books
            .Where(b => b.CategoryId == id)
            .ToListAsync();
        
        foreach (var book in booksInCategory)
        {
            book.CategoryId = uncategorized.Id;
            book.UpdatedAt = DateTime.UtcNow;
        }
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
