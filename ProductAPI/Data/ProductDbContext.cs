using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
       .Property(p => p.Id)
       .ValueGeneratedNever(); // Prevents ID updates

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Id)
            .IsUnique();

        modelBuilder.Entity<Product>()
       .Property(p => p.Price)
       .HasPrecision(18, 2); // 18 total digits, 2 decimal places
    }
}