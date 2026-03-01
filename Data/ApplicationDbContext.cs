using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookReservation.Models;

namespace BookReservation.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BorrowRequest> BorrowRequests { get; set; }
    public DbSet<Loan> Loans { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
        
        builder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<BorrowRequest>()
            .HasOne(br => br.Book)
            .WithMany(b => b.BorrowRequests)
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<BorrowRequest>()
            .HasOne(br => br.Student)
            .WithMany()
            .HasForeignKey(br => br.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<BorrowRequest>()
            .HasOne(br => br.ProcessedBy)
            .WithMany()
            .HasForeignKey(br => br.ProcessedById)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Loan>()
            .HasOne(l => l.Student)
            .WithMany()
            .HasForeignKey(l => l.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Loan>()
            .HasOne(l => l.BorrowRequest)
            .WithOne(br => br.Loan)
            .HasForeignKey<Loan>(l => l.BorrowRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
