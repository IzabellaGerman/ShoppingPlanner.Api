using Microsoft.AspNetCore.Mvc;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
    {
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
        {
        _productService = productService;
        }

    // GET api/products
    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> GetAll()
        {
        var products = _productService.GetAll();
        return Ok(products);
        }

    // GET api/products/5
    [HttpGet("{id:int}")]
    public ActionResult<ProductDto> GetById(int id)
        {
        var product = _productService.GetById(id);
        if (product is null)
            return NotFound();

        return Ok(product);
        }

    // POST api/products
    [HttpPost]
    public ActionResult<ProductDto> Create(CreateProductDto dto)
        {
        var product = _productService.Create(dto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
        }

    // PUT api/products/5
    [HttpPut("{id:int}")]
    public ActionResult<ProductDto> Update(int id, UpdateProductDto dto)
        {
        var product = _productService.Update(id, dto);
        if (product is null)
            return NotFound();

        return Ok(product);
        }

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        {
        var deleted = _productService.Delete(id);
        if (!deleted)
            return NotFound();

        return NoContent();
        }
    }