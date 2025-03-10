using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductAPI.Models;
using FluentAssertions;
public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithListOfProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 100001, Name = "Laptop", Price = 1200, Stock = 10 },
            new Product { Id = 100002, Name = "Mouse", Price = 50, Stock = 20 }
        };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 100001, Name = "Laptop", Price = 1200, Stock = 10 };
        _mockService.Setup(s => s.GetByIdAsync(100001)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(100001);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(999999)).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetById(999999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WithCreatedProduct()
    {
        // Arrange
        var newProduct = new Product { Name = "Keyboard", Price = 100, Stock = 5 };
        var createdProduct = new Product { Id = 100003, Name = "Keyboard", Price = 100, Stock = 5 };
        _mockService.Setup(s => s.AddAsync(newProduct)).ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.Create(newProduct);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProductController.GetById));
        createdResult.RouteValues["id"].Should().Be(createdProduct.Id);
        createdResult.Value.Should().BeEquivalentTo(createdProduct);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WithUpdatedProduct_WhenSuccessful()
    {
        // Arrange
        var updatedProduct = new Product { Id = 100001, Name = "Updated Laptop", Price = 1500, Stock = 5 };
        _mockService.Setup(s => s.UpdateAsync(100001, updatedProduct)).ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(100001, updatedProduct);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(updatedProduct);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var updatedProduct = new Product { Id = 100999, Name = "Updated Laptop", Price = 1500, Stock = 5 };
        _mockService.Setup(s => s.UpdateAsync(100999, updatedProduct)).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.UpdateProduct(100999, updatedProduct);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(100001)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(100001);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.DeleteAsync(999999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DecrementStock_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockService.Setup(s => s.DecrementStockAsync(100001, 2)).ReturnsAsync(true);

        // Act
        var result = await _controller.DecrementStock(100001, 2);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DecrementStock_ShouldReturnBadRequest_WhenStockIsInsufficient()
    {
        // Arrange
        _mockService.Setup(s => s.DecrementStockAsync(100001, 50)).ReturnsAsync(false);

        // Act
        var result = await _controller.DecrementStock(100001, 50);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddToStock_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        _mockService.Setup(s => s.AddToStockAsync(100001, 5)).ReturnsAsync(true);

        // Act
        var result = await _controller.AddToStock(100001, 5);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task AddToStock_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        _mockService.Setup(s => s.AddToStockAsync(100001, 5)).ReturnsAsync(false);

        // Act
        var result = await _controller.AddToStock(100001, 5);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }
}
