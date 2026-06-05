using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Services
    {
    public class CategoryService: ICategoryService 
        {

        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
            {
            _context = context;
            }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
            {
            var category = new Category { Name = dto.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
            }

        public async Task<bool> DeleteAsync(int id)
            {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return false; 
            
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return true;
            }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
            {             
            return await _context.Categories.
                Select(p => MapToDto(p))
                .ToListAsync();         
            }

        public async Task<CategoryDto?> GetByIdAsync(int id)
            {
            var category = await _context.Categories
            .FirstOrDefaultAsync(p => p.Id == id);

            return category is null ? null : MapToDto(category);
            }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
            {
            var category = await _context.Categories.FirstOrDefaultAsync(p => p.Id == id);
            if (category is null) return null;
            category.Name = dto.Name;
            await _context.SaveChangesAsync();
            return MapToDto(category);
            }

        private static CategoryDto MapToDto(Category category)
            {
            return new CategoryDto
                {
                Id = category.Id,
                Name = category.Name
                };
            }
            
        }
    }
