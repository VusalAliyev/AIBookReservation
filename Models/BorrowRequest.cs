namespace BookReservation.Models;

public class BorrowRequest
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public required string StudentId { get; set; }
    public ApplicationUser? Student { get; set; }
    public DateTime RequestedAt { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedById { get; set; }
    public ApplicationUser? ProcessedBy { get; set; }
    public string? Notes { get; set; }
    
    public Loan? Loan { get; set; }
}
