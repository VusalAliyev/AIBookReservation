using Microsoft.EntityFrameworkCore;
using BookReservation.Data;
using BookReservation.Models;

namespace BookReservation.Services;

public class LoanService : ILoanService
{
    private readonly ApplicationDbContext _context;
    
    public LoanService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Loan>> GetStudentActiveLoansAsync(string studentId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b!.Category)
            .Where(l => l.StudentId == studentId && l.ReturnedDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Loan>> GetStudentLoanHistoryAsync(string studentId)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b!.Category)
            .Where(l => l.StudentId == studentId && l.ReturnedDate != null)
            .OrderByDescending(l => l.ReturnedDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Loan>> GetAllLoansAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b!.Category)
            .Include(l => l.Student)
            .OrderByDescending(l => l.IssuedDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Loan>> GetActiveLoansAsync()
    {
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b!.Category)
            .Include(l => l.Student)
            .Where(l => l.ReturnedDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Loans
            .Include(l => l.Book)
            .ThenInclude(b => b!.Category)
            .Include(l => l.Student)
            .Where(l => l.ReturnedDate == null && l.DueDate < now)
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }
    
    public async Task<bool> ReturnLoanAsync(int loanId)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == loanId);
        
        if (loan == null || loan.ReturnedDate != null)
            return false;
        
        loan.ReturnedDate = DateTime.UtcNow;
        
        if (loan.Book != null)
        {
            loan.Book.AvailableCount++;
            loan.Book.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        return true;
    }
}
