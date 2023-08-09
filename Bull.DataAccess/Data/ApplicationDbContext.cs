using Bull.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bull.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
    {
    }

    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>()
            .HasData(
                new Category() { Id = 1, Name = "Action", DisplayOrder = 1},
                new Category() { Id = 2, Name = "Sci-Fi", DisplayOrder = 2},
                new Category() { Id = 3, Name = "History", DisplayOrder = 3}
                );
    }
}