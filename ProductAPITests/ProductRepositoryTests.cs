using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductAPI.Models;
using ProductAPI.Repositories;

public class ProductRepositoryTests
{
    private IMemoryCache _memoryCache;

    public ProductRepositoryTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        UniqueIdGenerator.Configure(_memoryCache); // Ensure cache is configured
    }

    private async Task<ProductDbContext> GetDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        var context = new ProductDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var context = await GetDbContextAsync();
        context.Products.AddRange(
            new Product { Id = 1, Name = "Laptop", Price = 1000, Stock = 5 },
            new Product { Id = 2, Name = "Phone", Price = 500, Stock = 10 }
        );
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var product = new Product { Id = 1, Name = "Laptop", Price = 1000, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(99);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct_WithUniqueId()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var repository = new ProductRepository(context);
        var product = new Product { Name = "Tablet", Price = 800, Stock = 3 };

        // Act
        var result = await repository.AddAsync(product);

        // Assert
        result.Id.Should().BeGreaterThan(100000).And.BeLessThan(999999);
        result.Name.Should().Be("Tablet");

        var savedProduct = await context.Products.FindAsync(result.Id);
        savedProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldNotOverwriteExistingId()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var repository = new ProductRepository(context);
        var product = new Product { Id = 123456, Name = "Monitor", Price = 200, Stock = 2 };

        // Act
        var result = await repository.AddAsync(product);

        // Assert
        result.Id.Should().Be(123456); // Should not be changed
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var product = new Product { Id = 1, Name = "Laptop", Price = 1000, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        product.Price = 1200;
        var updatedProduct = await repository.UpdateAsync(product);

        // Assert
        updatedProduct.Price.Should().Be(1200);
        var savedProduct = await context.Products.FindAsync(1);
        savedProduct!.Price.Should().Be(1200);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var context = await GetDbContextAsync();
        var product = new Product { Id = 1, Name = "Laptop", Price = 1000, Stock = 5 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        var deletedProduct = await context.Products.FindAsync(1);
        deletedProduct.Should().BeNull();
    }
}
