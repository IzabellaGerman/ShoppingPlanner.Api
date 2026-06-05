using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Services;

public class ProductService : IProductService
    {
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
        {
        _db = db;
        }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
        return await _db.Products
            .Include(p => p.Category)
            .Select(p => MapToDto(p))
            .ToListAsync();
        }

    public async Task<ProductDto?> GetByIdAsync(int id)
        {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product is null ? null : MapToDto(product);
        }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
        var product = new Product
            {
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            DefaultUnit = dto.DefaultUnit,
            CreatedAt = DateTime.UtcNow
            };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // Загружаем с Category для маппинга
        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return MapToDto(product);
        }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return null;

        product.Name = dto.Name;
        product.CategoryId = dto.CategoryId;
        product.DefaultUnit = dto.DefaultUnit;
        // CreatedAt не трогаем — см. INTERVIEW_NOTES

        await _db.SaveChangesAsync();

        // Перезагружаем Category если CategoryId изменился
        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return MapToDto(product);
        }

    public async Task<bool> DeleteAsync(int id)
        {
        var product = await _db.Products.FindAsync(id);

        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return true;
        }

    private static ProductDto MapToDto(Product product) => new ProductDto
        {
        Id = product.Id,
        Name = product.Name,
        CategoryId = product.CategoryId,
        CategoryName = product.Category?.Name,
        DefaultUnit = product.DefaultUnit,
        CreatedAt = product.CreatedAt
        };
    }