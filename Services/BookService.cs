using Microsoft.EntityFrameworkCore;
using BookReservation.Data;
using BookReservation.Models;

namespace BookReservation.Services;

public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;
    
    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .Include(b => b.Category)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId)
    {
        var query = _context.Books.Include(b => b.Category).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b => 
                b.Title.Contains(searchTerm) || 
                b.Author.Contains(searchTerm));
        }
        
        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }
        
        return await query.OrderBy(b => b.Title).ToListAsync();
    }
    
    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    
    public async Task<Book> CreateBookAsync(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = DateTime.UtcNow;
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        
        return book;
    }
    
    public async Task<Book?> UpdateBookAsync(Book book)
    {
        var existing = await _context.Books.FindAsync(book.Id);
        if (existing == null)
            return null;
        
        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.Description = book.Description;
        existing.ISBN = book.ISBN;
        existing.CategoryId = book.CategoryId;
        existing.TotalCount = book.TotalCount;
        existing.AvailableCount = book.AvailableCount;
        existing.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return existing;
    }
    
    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;
        
        var hasActiveLoans = await _context.Loans
            .AnyAsync(l => l.BookId == id && l.ReturnedDate == null);
        
        if (hasActiveLoans)
            return false;
        
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        
        return true;
    }
}
