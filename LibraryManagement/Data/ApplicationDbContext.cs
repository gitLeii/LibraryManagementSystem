using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<BooksPartial> AllBooks { get; set; }
        public DbSet<ReserveSettings> Settings { get; set; }
        
    }
}