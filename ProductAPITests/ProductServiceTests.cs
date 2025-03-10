using FluentAssertions;
using Moq;
using ProductAPI.Models;
using ProductAPI.Repositories;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product A", Price = 100, Stock = 10 },
            new() { Id = 2, Name = "Product B", Price = 200, Stock = 20 }
        };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(products);
        _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product A", Price = 100, Stock = 10 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(product);
        _mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product)null);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldReturnAddedProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "New Product", Price = 150, Stock = 5 };
        _mockRepo.Setup(repo => repo.AddAsync(product)).ReturnsAsync(product);

        // Act
        var result = await _service.AddAsync(product);

        // Assert
        result.Should().BeEquivalentTo(product);
        _mockRepo.Verify(repo => repo.AddAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedProduct_WhenExists()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 100, Stock = 10 };
        var updatedProduct = new Product { Id = 1, Name = "New Name", Price = 150, Stock = 5 };

        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(updatedProduct);

        // Act
        var result = await _service.UpdateAsync(1, updatedProduct);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("New Name");
        result?.Price.Should().Be(150);
        result?.Stock.Should().Be(5);

        _mockRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenProductNotExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product)null);

        // Act
        var result = await _service.UpdateAsync(1, new Product { Name = "New Name" });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DecrementStockAsync_ShouldReturnTrue_WhenStockIsSufficient()
    {
        // Arrange
        var product = new Product { Id = 1, Stock = 10 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);
        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _service.DecrementStockAsync(1, 5);

        // Assert
        result.Should().BeTrue();
        product.Stock.Should().Be(5);
    }

    [Fact]
    public async Task DecrementStockAsync_ShouldReturnFalse_WhenStockIsInsufficient()
    {
        // Arrange
        var product = new Product { Id = 1, Stock = 3 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.DecrementStockAsync(1, 5);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddToStockAsync_ShouldIncreaseStock()
    {
        // Arrange
        var product = new Product { Id = 1, Stock = 5 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);
        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _service.AddToStockAsync(1, 10);

        // Assert
        result.Should().BeTrue();
        product.Stock.Should().Be(15);
    }
}
