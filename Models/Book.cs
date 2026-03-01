namespace BookReservation.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int TotalCount { get; set; }
    public int AvailableCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<BorrowRequest> BorrowRequests { get; set; } = new List<BorrowRequest>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
