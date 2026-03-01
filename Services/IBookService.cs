using BookReservation.Models;

namespace BookReservation.Services;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm, int? categoryId);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> CreateBookAsync(Book book);
    Task<Book?> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(int id);
    IQueryable<Book> GetBooksQueryable();
}
