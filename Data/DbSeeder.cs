using Microsoft.AspNetCore.Identity;
using BookReservation.Models;

namespace BookReservation.Data;

public class DbSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public DbSeeder(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedCategoriesAsync();
        await SeedBooksAsync();
    }
    
    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Student", "Librarian" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    private async Task SeedUsersAsync()
    {
        var librarianEmail = "librarian@school.com";
        if (await _userManager.FindByEmailAsync(librarianEmail) == null)
        {
            var librarian = new ApplicationUser
            {
                UserName = librarianEmail,
                Email = librarianEmail,
                FullName = "System Librarian",
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(librarian, "Librarian123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(librarian, "Librarian");
            }
        }
        
        var studentEmail = "student@school.com";
        if (await _userManager.FindByEmailAsync(studentEmail) == null)
        {
            var student = new ApplicationUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                FullName = "Test Student",
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(student, "Student123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(student, "Student");
            }
        }
    }
    
    private async Task SeedCategoriesAsync()
    {
        if (!_context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "Uncategorized", Description = "Books without a specific category", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "Fiction", Description = "Fiction books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "Science", Description = "Science books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "History", Description = "History books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Category { Name = "Technology", Description = "Technology books", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            
            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }
    }
    
    private async Task SeedBooksAsync()
    {
        if (!_context.Books.Any())
        {
            var fiction = _context.Categories.First(c => c.Name == "Fiction");
            var science = _context.Categories.First(c => c.Name == "Science");
            var history = _context.Categories.First(c => c.Name == "History");
            var technology = _context.Categories.First(c => c.Name == "Technology");
            
            var books = new[]
            {
                new Book { Title = "To Kill a Mockingbird", Author = "Harper Lee", Description = "A classic American novel", ISBN = "978-0061120084", CategoryId = fiction.Id, TotalCount = 5, AvailableCount = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Book { Title = "1984", Author = "George Orwell", Description = "Dystopian social science fiction", ISBN = "978-0451524935", CategoryId = fiction.Id, TotalCount = 3, AvailableCount = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Book { Title = "A Brief History of Time", Author = "Stephen Hawking", Description = "Science book about cosmology", ISBN = "978-0553380163", CategoryId = science.Id, TotalCount = 4, AvailableCount = 4, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Description = "A brief history of humankind", ISBN = "978-0062316097", CategoryId = history.Id, TotalCount = 6, AvailableCount = 6, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Book { Title = "Clean Code", Author = "Robert C. Martin", Description = "A handbook of agile software craftsmanship", ISBN = "978-0132350884", CategoryId = technology.Id, TotalCount = 2, AvailableCount = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Description = "Your journey to mastery", ISBN = "978-0135957059", CategoryId = technology.Id, TotalCount = 3, AvailableCount = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
        }
    }
}
