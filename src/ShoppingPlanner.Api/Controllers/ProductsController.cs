using Microsoft.AspNetCore.Mvc;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;
using Microsoft.AspNetCore.Http;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ProductDto>> GetAll()
        {
        var products = _productService.GetAll();
        return Ok(products);
        }

    // GET api/products/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> GetById(int id)
        {
        var product = _productService.GetById(id);
        if (product is null)
            return NotFound();

        return Ok(product);
        }

    // POST api/products
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> Update(int id, UpdateProductDto dto)
        {
        var product = _productService.Update(id, dto);
        if (product is null)
            return NotFound();

        return Ok(product);
        }

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
        {
        var deleted = _productService.Delete(id);
        if (!deleted)
            return NotFound();

        return NoContent();
        }
    }