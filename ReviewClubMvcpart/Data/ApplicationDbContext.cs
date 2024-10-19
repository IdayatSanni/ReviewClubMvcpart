using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews) // Navigation property in Book
                .HasForeignKey(r => r.BookId)
                .IsRequired(); // Ensure Book cannot be null

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(re => re.Reviews) // Navigation property in Reviewer
                .HasForeignKey(r => r.ReviewersId)
                .IsRequired(); // Ensure Reviewer cannot be null

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if needed
        }
    }
}
