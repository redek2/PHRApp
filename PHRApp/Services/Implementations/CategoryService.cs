using Microsoft.EntityFrameworkCore;
using PHRApp.Data;
using PHRApp.Models.DTOs;
using PHRApp.Models.Entities;
using PHRApp.Services.Interfaces;

namespace PHRApp.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Category name is required.");
            }

            var trimmedName = dto.Name.Trim();
            var normalizeName = trimmedName.ToLowerInvariant();

            var exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == normalizeName);

            if (exists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            var category = new Category
            {
                Name = trimmedName,
                Description = dto.Description
            };



            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
