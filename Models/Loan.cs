namespace BookReservation.Models;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public required string StudentId { get; set; }
    public ApplicationUser? Student { get; set; }
    public int BorrowRequestId { get; set; }
    public BorrowRequest? BorrowRequest { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    
    public bool IsReturned => ReturnedDate.HasValue;
    public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;
}
