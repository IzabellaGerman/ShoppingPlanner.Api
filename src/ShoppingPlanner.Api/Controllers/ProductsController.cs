using Microsoft.AspNetCore.Mvc;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
    {
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
        _productService = productService;
        _logger = logger;
        }

    // GET api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
        var products = await _productService.GetAllAsync();
        return Ok(products);
        }

    // GET api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
        {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            {
            _logger.LogWarning("Product {Id} not found", id);
            return NotFound();
            }

        return Ok(product);
        }

    // POST api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
        {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

    // PUT api/products/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto dto)
        {
        var product = await _productService.UpdateAsync(id, dto);

        if (product is null) return NotFound();

        return Ok(product);
        }

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted) return NotFound();

        return NoContent();
        }
    }