using Microsoft.AspNetCore.Identity;

namespace BookReservation.Models;

public class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }
}
