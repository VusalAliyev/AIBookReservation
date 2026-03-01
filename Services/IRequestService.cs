using BookReservation.Models;

namespace BookReservation.Services;

public interface IRequestService
{
    Task<BorrowRequest?> CreateRequestAsync(int bookId, string studentId);
    Task<IEnumerable<BorrowRequest>> GetStudentRequestsAsync(string studentId);
    Task<IEnumerable<BorrowRequest>> GetAllRequestsAsync();
    Task<IEnumerable<BorrowRequest>> GetPendingRequestsAsync();
    Task<bool> CancelRequestAsync(int requestId, string studentId);
    Task<bool> AcceptRequestAsync(int requestId, string librarianId);
    Task<bool> DenyRequestAsync(int requestId, string librarianId);
    Task<bool> CanStudentCreateRequestAsync(string studentId, int bookId);
}
