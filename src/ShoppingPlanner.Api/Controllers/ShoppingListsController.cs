using Microsoft.AspNetCore.Mvc;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShoppingPlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingListsController : ControllerBase
    {
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
        {
        _shoppingListService = shoppingListService;
        }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetAll()
        {
        var lists = await _shoppingListService.GetAllAsync();
        return Ok(lists);
        }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> GetById(int id)
        {
        var list = await _shoppingListService.GetByIdAsync(id);
        if (list is null)
            {
            return NotFound();
            }

        return Ok(list);
        }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> Create(CreateShoppingListDto dto)
        {
        var created = await _shoppingListService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> Update(int id, UpdateShoppingListDto dto)
        {
        var updated = await _shoppingListService.UpdateAsync(id, dto);
        if (updated is null)
            {
            return NotFound();
            }

        return Ok(updated);
        }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        {
        var deleted = await _shoppingListService.DeleteAsync(id);
        if (!deleted)
            {
            return NotFound();
            }

        return NoContent();
        }

    [HttpPost("{id:int}/items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListItemDto>> AddItem(int id, CreateShoppingListItemDto dto)
        {
        var added = await _shoppingListService.AddItemAsync(id, dto);
        if (added is null)
            {
            return NotFound();
            }

        return Created($"/api/shoppinglists/{id}/items/{added.Id}", added);

        }

    [HttpPatch("{id:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListItemDto>> UpdateItem(int id, int itemId, UpdateShoppingListItemDto dto)
        {
        var updated = await _shoppingListService.UpdateItemAsync(id, itemId, dto);
        if (updated is null)
            {
            return NotFound();
            }

        return Ok(updated);
        }

    [HttpDelete("{id:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(int id, int itemId)
        {
        var deleted = await _shoppingListService.RemoveItemAsync(id, itemId);
        if (!deleted)
            {
            return NotFound();
            }

        return NoContent();

        }
    }