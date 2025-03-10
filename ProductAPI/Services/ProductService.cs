using ProductAPI.Models;
using ProductAPI.Repositories;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<Product> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<Product> AddAsync(Product product) => await _repository.AddAsync(product);

    public async Task<Product?> UpdateAsync(int id, Product updatedProduct)
    {
        var existingProduct = await _repository.GetByIdAsync(id);
        if (existingProduct == null) return null;

        // Update only allowed fields (excluding Id)
        existingProduct.Name = updatedProduct.Name;
        existingProduct.Price = updatedProduct.Price;
        existingProduct.Stock = updatedProduct.Stock;

        return await _repository.UpdateAsync(existingProduct);
    }

    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<bool> DecrementStockAsync(int id, int quantity)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null || product.Stock < quantity) return false;
        product.Stock -= quantity;
        await _repository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> AddToStockAsync(int id, int quantity)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return false;
        product.Stock += quantity;
        await _repository.UpdateAsync(product);
        return true;
    }
}
