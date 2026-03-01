using System.ComponentModel.DataAnnotations;

namespace BookReservation.ViewModels;

public class BookViewModel
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Author { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? ISBN { get; set; }
    
    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Total count must be 0 or greater")]
    [Display(Name = "Total Count")]
    public int TotalCount { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Available count must be 0 or greater")]
    [Display(Name = "Available Count")]
    public int AvailableCount { get; set; }
}
