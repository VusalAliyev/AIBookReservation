using BookReservation.Models;

namespace BookReservation.Services;

public interface ILoanService
{
    Task<IEnumerable<Loan>> GetStudentActiveLoansAsync(string studentId);
    Task<IEnumerable<Loan>> GetStudentLoanHistoryAsync(string studentId);
    Task<IEnumerable<Loan>> GetAllLoansAsync();
    Task<IEnumerable<Loan>> GetActiveLoansAsync();
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<bool> ReturnLoanAsync(int loanId);
}
