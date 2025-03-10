using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();

    public async Task<Product> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task<Product> AddAsync(Product product)
    {
        if (product.Id == 0) // Generate ID only for new products
        {
            product.Id = UniqueIdGenerator.GenerateUniqueId(_context);
        }
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
