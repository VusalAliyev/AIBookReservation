using Microsoft.EntityFrameworkCore;
using BookReservation.Data;
using BookReservation.Models;

namespace BookReservation.Services;

public class RequestService : IRequestService
{
    private readonly ApplicationDbContext _context;
    
    public RequestService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CanStudentCreateRequestAsync(string studentId, int bookId)
    {
        var hasPendingRequest = await _context.BorrowRequests
            .AnyAsync(br => br.StudentId == studentId && br.Status == RequestStatus.Pending);
        
        if (hasPendingRequest)
            return false;
        
        var hasActiveLoan = await _context.Loans
            .AnyAsync(l => l.StudentId == studentId && l.BookId == bookId && l.ReturnedDate == null);
        
        if (hasActiveLoan)
            return false;
        
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.AvailableCount <= 0)
            return false;
        
        return true;
    }
    
    public async Task<BorrowRequest?> CreateRequestAsync(int bookId, string studentId)
    {
        if (!await CanStudentCreateRequestAsync(studentId, bookId))
            return null;
        
        var request = new BorrowRequest
        {
            BookId = bookId,
            StudentId = studentId,
            RequestedAt = DateTime.UtcNow,
            Status = RequestStatus.Pending
        };
        
        _context.BorrowRequests.Add(request);
        await _context.SaveChangesAsync();
        
        return request;
    }
    
    public async Task<IEnumerable<BorrowRequest>> GetStudentRequestsAsync(string studentId)
    {
        return await _context.BorrowRequests
            .Include(br => br.Book)
            .ThenInclude(b => b!.Category)
            .Where(br => br.StudentId == studentId)
            .OrderByDescending(br => br.RequestedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<BorrowRequest>> GetAllRequestsAsync()
    {
        return await _context.BorrowRequests
            .Include(br => br.Book)
            .ThenInclude(b => b!.Category)
            .Include(br => br.Student)
            .OrderByDescending(br => br.RequestedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<BorrowRequest>> GetPendingRequestsAsync()
    {
        return await _context.BorrowRequests
            .Include(br => br.Book)
            .ThenInclude(b => b!.Category)
            .Include(br => br.Student)
            .Where(br => br.Status == RequestStatus.Pending)
            .OrderBy(br => br.RequestedAt)
            .ToListAsync();
    }
    
    public async Task<bool> CancelRequestAsync(int requestId, string studentId)
    {
        var request = await _context.BorrowRequests
            .FirstOrDefaultAsync(br => br.Id == requestId && br.StudentId == studentId);
        
        if (request == null || request.Status != RequestStatus.Pending)
            return false;
        
        request.Status = RequestStatus.Cancelled;
        request.ProcessedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> AcceptRequestAsync(int requestId, string librarianId)
    {
        var request = await _context.BorrowRequests
            .Include(br => br.Book)
            .FirstOrDefaultAsync(br => br.Id == requestId);
        
        if (request == null || request.Status != RequestStatus.Pending)
            return false;
        
        if (request.Book == null || request.Book.AvailableCount <= 0)
            return false;
        
        var hasPendingRequest = await _context.BorrowRequests
            .AnyAsync(br => br.StudentId == request.StudentId && 
                           br.Id != requestId && 
                           br.Status == RequestStatus.Pending);
        
        if (hasPendingRequest)
            return false;
        
        var hasActiveLoan = await _context.Loans
            .AnyAsync(l => l.StudentId == request.StudentId && 
                          l.BookId == request.BookId && 
                          l.ReturnedDate == null);
        
        if (hasActiveLoan)
            return false;
        
        request.Status = RequestStatus.Accepted;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedById = librarianId;
        
        request.Book.AvailableCount--;
        request.Book.UpdatedAt = DateTime.UtcNow;
        
        var issuedDate = DateTime.UtcNow;
        var loan = new Loan
        {
            BookId = request.BookId,
            StudentId = request.StudentId,
            BorrowRequestId = request.Id,
            IssuedDate = issuedDate,
            DueDate = issuedDate.AddDays(7)
        };
        
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DenyRequestAsync(int requestId, string librarianId)
    {
        var request = await _context.BorrowRequests.FindAsync(requestId);
        
        if (request == null || request.Status != RequestStatus.Pending)
            return false;
        
        request.Status = RequestStatus.Denied;
        request.ProcessedAt = DateTime.UtcNow;
        request.ProcessedById = librarianId;
        
        await _context.SaveChangesAsync();
        
        return true;
    }
}
