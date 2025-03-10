using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product != null ? Ok(product) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        var createdProduct = await _service.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        if (updatedProduct == null)
            return BadRequest("Product data is required.");

        if (updatedProduct.Id != 0 && updatedProduct.Id != id)
            return BadRequest("Updating 'Id' is not allowed.");

        var updated = await _service.UpdateAsync(id, updatedProduct);

        return updated is not null ? Ok(updated) : NotFound($"Product with ID {id} not found.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? Ok() : NotFound();

    [HttpPut("decrement-stock/{id}/{quantity}")]
    public async Task<IActionResult> DecrementStock(int id, int quantity) =>
        await _service.DecrementStockAsync(id, quantity) ? Ok() : BadRequest("Insufficient stock");

    [HttpPut("add-to-stock/{id}/{quantity}")]
    public async Task<IActionResult> AddToStock(int id, int quantity) =>
        await _service.AddToStockAsync(id, quantity) ? Ok() : BadRequest();
}
