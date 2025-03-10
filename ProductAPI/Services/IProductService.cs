using ProductAPI.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(int id, Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> DecrementStockAsync(int id, int quantity);
    Task<bool> AddToStockAsync(int id, int quantity);
}
