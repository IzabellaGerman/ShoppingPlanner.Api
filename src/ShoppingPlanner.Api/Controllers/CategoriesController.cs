using Microsoft.AspNetCore.Mvc;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;


namespace ShoppingPlanner.Api.Controllers
    {
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
        {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
            {
            _categoryService = categoryService;
            }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            {
            var category = await _categoryService.GetAllAsync();
            return Ok(category);
            }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) {

            var category = await _categoryService.GetByIdAsync(id);
            if (category is null) return NotFound();
            return Ok(category);
            }

        [HttpPost]

        public async Task<IActionResult> Create(CreateCategoryDto dto) {

            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);

            }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto) {
            
            var category = await _categoryService.UpdateAsync(id, dto);
            if (category is null) return NotFound();
            return Ok(category);
            }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) {
           
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted) return NotFound();

            return NoContent();
            }
        }
    }
